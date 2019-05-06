using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;

namespace CognitoLoginExample
{
    public struct TogetherUser
    {
        public string Password;
        public string Username;
        public string Email;
        public string LastName;
        public string FirstName;
        public string BirthDate;
        public string ChurchUsername;
        public UserStatusType Status { get; set; }
    }
}
