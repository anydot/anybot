namespace RoumenBot.Tag
{
    public sealed class Maso : TagBase
    {
        protected override string DivNameInternal => "masoList";

        protected override int ImageTdIndexInternal => 6;

        protected override string ShowPrefixInternal => "/masoShow.php?file=";
    }

}