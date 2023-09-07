namespace RandomMovie
{
    internal class Movie
    {
        public int SortValue { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string LetterboxdURI { get; set; }
        public string FileName { get; set; }
        public string PosterWebsiteLink { get; set; }
        public System.Drawing.Color MainColor { get; set; }
        public bool IsDark { get; set; }
        public string FilmID { get; set; }

        public Color MainColorMaui => GetColor(MainColor);

        private Color GetColor(System.Drawing.Color mainColor)
        {
            return Color.FromRgba(mainColor.R, mainColor.G, mainColor.B, mainColor.A);
        }
    }
}
