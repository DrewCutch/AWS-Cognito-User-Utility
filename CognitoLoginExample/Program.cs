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
                try
                {
                    Console.WriteLine("Enter your credentials");
                    Console.Write("Username: ");
                    string username = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
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
            /*
            Console.WriteLine("Enter Fields:");
            string birthdate = Prompt("Birthdate");
            string churchUsername = Prompt("Church Username");
            string email = Prompt("Email");
            string firstName = Prompt("First Name");
            */
            TogetherUser newUser = new TogetherUser
            {
                BirthDate = "1999-08-18",
                ChurchUsername = "NPCChurch3",
                Email = "drew.cutch@gmail.com",
                FirstName = "Andrew",
                LastName = "Cutchins",
                Password = "Test1234",
                Status = new UserStatusType("test"),
                Username = "DrewCutch"
            };
            Task signUpTask = AccountUtils.CreateAsync(newUser);
            signUpTask.Wait();
            PrintSuccess("Account successfully created");
            Console.ReadLine();
        }
    }

    
}
