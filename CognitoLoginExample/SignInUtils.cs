using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;

namespace CognitoLoginExample
{
    internal static class AccountUtils
    {
        private static readonly string PoolID;
        private static readonly string ClientID;
        private static readonly AmazonCognitoIdentityProviderClient provider;
        static AccountUtils()
        {
            AWSConfigs.AWSRegion = "us-east-2";
            PoolID = ConfigurationManager.AppSettings["cognitoPoolID"];
            ClientID = ConfigurationManager.AppSettings["cognitoClientID"];
            provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());
        }

        public static async Task<string> GetCredsAsync(string username, string password)
        {
            
            CognitoUserPool userPool = new CognitoUserPool(PoolID, ClientID, provider);
            CognitoUser user = new CognitoUser(username, ClientID, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            string idToken = null;

            while (authResponse.AuthenticationResult == null)
            {
                if (authResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
                {
                    Console.WriteLine("Your account requires a new password");
                    Console.WriteLine("Enter your desired new password:");
                    string newPassword = Console.ReadLine();

                    authResponse = await user.RespondToNewPasswordRequiredAsync(
                        new RespondToNewPasswordRequiredRequest()
                        {
                            SessionID = authResponse.SessionID,
                            NewPassword = newPassword
                        });
                    idToken = authResponse.AuthenticationResult.IdToken;
                }
                else if (authResponse.ChallengeName == ChallengeNameType.SMS_MFA)
                {
                    Console.WriteLine("Enter the MFA Code sent to your device:");
                    string mfaCode = Console.ReadLine();

                    AuthFlowResponse mfaResponse = await user.RespondToSmsMfaAuthAsync(new RespondToSmsMfaRequest()
                    {
                        SessionID = authResponse.SessionID,
                        MfaCode = mfaCode

                    }).ConfigureAwait(false);
                    idToken = mfaResponse.AuthenticationResult.IdToken;
                }
                else
                {
                    Console.WriteLine("Unrecognized authentication challenge.");
                    idToken = null;
                    break;
                }
            }

            return idToken ?? authResponse.AuthenticationResult?.IdToken;
        }

        public static Task CreateAsync(TogetherUser user)
        {
            // Register the user using Cognito
            var signUpRequest = new SignUpRequest
            {
                ClientId = ClientID,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            var birthdateAttribute = new AttributeType
            {
                Name = "birthdate",
                Value = user.BirthDate
            };
            var familyNameAttribute = new AttributeType
            {
                Name = "family_name",
                Value = user.LastName
            };
            var givenNameAttribute = new AttributeType
            {
                Name = "given_name",
                Value = user.FirstName
            };
            var churchUsernameAttribute = new AttributeType
            {
                Name = "custom:churchUsernam",
                Value = user.ChurchUsername
            };

            signUpRequest.UserAttributes.Add(emailAttribute);
            signUpRequest.UserAttributes.Add(birthdateAttribute);
            signUpRequest.UserAttributes.Add(familyNameAttribute);
            signUpRequest.UserAttributes.Add(givenNameAttribute);
            signUpRequest.UserAttributes.Add(churchUsernameAttribute);

            return provider.SignUpAsync(signUpRequest);
        }
    }
}
