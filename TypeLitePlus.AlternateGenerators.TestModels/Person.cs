using System.Collections.Generic;

namespace TypeLitePlus.AlternateGenerators.TestModels
{
    public class Person
    {
        public const int MaxAddresses = 3;
        public const string DefaultPhoneNumber = "[None]";

        public string PhoneNumber;

        public string Name { get; set; }
        public int YearOfBirth { get; set; }

        public Address PrimaryAddress { get; set; }
        public List<Address> Addresses { get; set; }
    }
}
