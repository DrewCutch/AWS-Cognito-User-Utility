using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using static CognitoLoginExample.ConsoleHelper;

namespace CognitoLoginExample
{
    internal class Program
    {
        
        private static void Main(string[] args)
        {
            while (true)
            {
                int choice = ConsoleHelper.MultipleChoice("Select Action", "Create Account", "Login", "Quit");
                Console.Clear();
                switch (choice)
                {
                    case 0:
                    {
                        CreateAccount();
                        break;
                    }
                    case 1:
                    {
                        Login();
                        break;
                    }
                    case 2:
                    {
                        return;
                    }
                }
            }


        }

        private static void Login()
        {
            while(true)
            {
                Console.WriteLine("Enter your credentials");
                string username = Prompt("Username");
                string password = Prompt("Password");
                try
                {
                    Task<string> signInTask = AccountUtils.GetCredsAsync(username, password);
                    signInTask.Wait();
                    string idToken = signInTask.Result;
                    PrintSuccess("Log-in successful!");
                    Console.WriteLine("idToken:");
                    Console.WriteLine(idToken);
                    break;
                }
                catch (AggregateException e)
                {
                    foreach (Exception innerException in e.InnerExceptions)
                    {
                        switch (innerException)
                        {
                            case UserNotFoundException _:
                                PrintError("User does not exist!");
                                break;
                            case NotAuthorizedException _:
                                PrintError("Incorrect password!");
                                break;
                            case UserNotConfirmedException _:
                                PrintError("User is not confirmed!");
                                break;
                            default:
                                throw innerException;
                        }
                    }
                    if(!ConsoleHelper.TryAgain())
                        return;
                }

            }
            Console.ReadLine();
        }

        private static void CreateAccount()
        {
            
            Console.WriteLine("Enter Fields:");
            string username = Prompt("Username");
            string password = Prompt("Password");
            string email = Prompt("Email");
            string firstName = Prompt("First Name");
            string lastName = Prompt("Last Name");
            string birthdate = Prompt("Birthdate");
            string churchUsername = Prompt("Church Username");

            TogetherUser newUser = new TogetherUser
            {
                BirthDate = birthdate,
                ChurchUsername = churchUsername,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                Status = new UserStatusType("test"),
                Username = username
            };
            Task signUpTask = AccountUtils.CreateAsync(newUser);
            signUpTask.Wait();
            PrintSuccess("Account successfully created");
            Console.ReadLine();
        }
    }

    
}
