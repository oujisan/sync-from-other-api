namespace syncdata.Models
{
    public class Character
    {
        public int CharId { get; set; }
        public string Name { get; set; }
        public int Rarity { get; set; }
        public string Path { get; set; }
        public string Element { get; set; }
        public DateTime Release { get; set; }
        public string Introduction { get; set; }
        public string ImgUrl { get; set; }
    }
}