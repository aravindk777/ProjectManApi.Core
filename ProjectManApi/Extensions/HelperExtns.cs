namespace PM.Api.Extensions
{
    /// <summary>
    /// Helper method to create JSON string from the object
    /// </summary>
    public static class HelperExtns
    {
        /// <summary>
        /// Stringify method
        /// </summary>
        /// <param name="input">any object</param>
        /// <returns>string</returns>
        public static string Stringify(this object input)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(input);
        }
    }
}