namespace RandomMovie
{
    internal class Movie
    {
        public int SortValue;
        public DateTime Date;
        public string Name { get; set; }
        public int Year;
        public string LetterboxdURI;
        public double Rating;
        public string Tags;
        public string FileName;
        public string PosterWebsiteLink { get; set; }
        public string IMBD;
        public string TMDB;
        public System.Drawing.Color MainColor { get; set; }
        public bool IsDark;
        public string FilmID;

        public Color MainColorMaui => GetColor(MainColor);

        private Color GetColor(System.Drawing.Color mainColor)
        {
            return Color.FromRgba(mainColor.R, mainColor.G, mainColor.B, mainColor.A);
        }
    }
}
