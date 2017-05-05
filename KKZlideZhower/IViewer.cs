namespace KKZlideZhower
{
    internal interface IViewer
    {
        void view();
        System.TimeSpan time { get; set; }
    }
}