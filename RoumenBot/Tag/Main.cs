namespace RoumenBot.Tag
{
    public sealed class Main : TagBase
    {
        protected override string DivNameInternal => "middle";

        protected override int ImageTdIndexInternal => 7;

        protected override string ShowPrefixInternal => "/roumingShow.php?file=";
    }

}