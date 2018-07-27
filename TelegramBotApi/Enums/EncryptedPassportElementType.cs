namespace TelegramBotApi.Enums
{
    /*
     * 
     * IF YOU ADD A TYPE HERE, ADD IT IN EncryptedPassportElement.cs AS WELL!!!!!!!!
     * ADD IT IN BOTH THE GETTER AND THE SETTER OF Type!
     * 
     */

    /// <summary>
    /// Type of an Encrypted Passport Element
    /// </summary>
    public enum EncryptedPassportElementType
    {
        /// <summary>
        /// Personal Details
        /// </summary>
        PersonalDetails,
        /// <summary>
        /// Passport
        /// </summary>
        Passport,
        /// <summary>
        /// Driver License
        /// </summary>
        DriverLicense,
        /// <summary>
        /// Identity Card
        /// </summary>
        IdentityCard,
        /// <summary>
        /// Internal Passport
        /// </summary>
        InternalPassport,
        /// <summary>
        /// Address
        /// </summary>
        Address,
        /// <summary>
        /// Utility Bill
        /// </summary>
        UtilityBill,
        /// <summary>
        /// Bank Statement
        /// </summary>
        BankStatement,
        /// <summary>
        /// Rental Agreement
        /// </summary>
        RentalAgreement,
        /// <summary>
        /// Passport Registration
        /// </summary>
        PassportRegistration,
        /// <summary>
        /// Temporary Registration
        /// </summary>
        TemporaryRegistration,
        /// <summary>
        /// Phone Number
        /// </summary>
        PhoneNumber,
        /// <summary>
        /// Email
        /// </summary>
        Email,
        /// <summary>
        /// An encrypted passport element type that isn't implemented yet
        /// </summary>
        Unknown
    }
}
