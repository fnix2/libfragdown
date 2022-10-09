namespace libfragdown
{
    public static class SetOfSitesHandlers
    {
        public static LibfragdownStartProcess ImageExtractorGeneric(Uri url)
        {
            return new LibfragdownStartProcess(url);
        }
        /* public static ImageExtractor ImageExtractorGeneric() */
    }
}
