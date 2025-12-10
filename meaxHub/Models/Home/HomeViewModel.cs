namespace MeaxHub.Models.Home
{
    public class HomeSystemItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
    }

    public class HomeViewModel
    {
        public List<HomeSystemItem> Systems { get; set; } = new();
    }
}
