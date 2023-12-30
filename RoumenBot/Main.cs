namespace RoumenBot
{
    namespace Tag
    {
        public struct Main : ITag
        {
            public static string ShowPrefix => "/roumingShow.php?file=";

            public static string ImageLinkNodeXPath => "//div[contains(@class, 'roumingList')]/div/div/table[1]/tr/td[last()]/a";
        }
    }
}