namespace RoumenBot
{
    namespace Tag
    {
        public struct Maso : ITag
        {
            public static string ShowPrefix => "/masoShow.php?file=";

            public static string ImageLinkNodeXPath => "//div[contains(@class, 'masoList')]/table[1]/tr/td/table[1]/tr/td[last()]/a";
        }
    }
}