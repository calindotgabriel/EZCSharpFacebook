// Created by Călin Gabriel
// at 21:10 on 27/02/2016.
//  

using System;

namespace EZCSharpFacebook.Models
{
    public class User
    {

        public string FacebookID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Name { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string Bio { get; set; }
        public string About { get; set; }
        public string AgeRangeMin { get; set; }
        public string AgeRangeMax { get; set; }

        public override string ToString()
        {
            return String.Join("**", FacebookID, FirstName, LastName, Name, Gender);
        }
    }
}