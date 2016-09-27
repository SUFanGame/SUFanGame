namespace StevenUniverse.FanGame.Util
{
    public class Gender : EnhancedEnum<Gender>
    {
        //Instance
        private Gender(string name) : base(name)
        {
        }

        //Static Instances
        static Gender()
        {
            Add(new Gender("None"));
            Add(new Gender("Male"));
            Add(new Gender("Female"));
        }
    }
}