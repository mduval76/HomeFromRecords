namespace HomeFromRecords.Core.Data {
    public class Constants {
        //==================( ALBUM )==================
        public enum Grade {
            M,
            NM,
            VG_PLUS,
            VG,
            G_PLUS,
            G,
            F,
            P,
            UNSPECIFIED
        }

        public enum MainFormat {
            VINYL,
            CD,
            CASSETTE,
            UNSPECIFIED
        }

        public enum SubFormat {
            STANDARD, // No special features
            TWELVE_INCH,
            TEN_INCH,
            SEVEN_INCH,
            PICTURE_DISC,
            ACETATE,
            LATHE,
            FLEXI,
            THREE_INCH,
            CD_R,
            DVD,
            BLURAY,
            LASERDISC,
            REEL_TO_REEL,
            EIGHT_TRACK,
            UNSPECIFIED
        }

        public enum PackageType {
            STANDARD,  // No special features
            GATEFOLD,
            GENERIC,
            DIGIPAK,
            CASING, // Jewel case, cassette case, etc.
            BOX,
            OVERSIZED,
            SLIPCASE,
            HOMEMADE,
            CUSTOM,
            PVC,
            NONE,
            UNSPECIFIED
        }

        public enum VinylSpeed { // Add for vinyl only
            THIRTY_THREE,
            FORTY_FIVE,
            SEVENTY_EIGHT,
            VARIOUS,
            INNAPLICABLE, // Automatically set for non-vinyl
            UNSPECIFIED
        }

        public enum AlbumLength {
            LP,
            EP,
            SINGLE,
            DOUBLE,
            TRIPLE,
            MULTIPLE,
            BOXSET,
            UNSPECIFIED
        }

        public enum AlbumType {
            STUDIO,
            LIVE,
            COMPILATION,    // Same artist, "Best Of" or "Greatest Hits"
            VARIOUS,        // Different artists
            MIXTAPE,
            SPLIT,
            SOUNDTRACK,
            UNSPECIFIED
        }

        public enum AlbumGenre {
            AFROBEAT, ALTERNATIVE, AMBIENT, ASIA, BAROQUE, BEBOP,
            BLACK, BLUES, BREAKBEAT, CARIBBEAN, CHAMBER, CHICAGO,
            CHORAL, CHRISTMAS, CLASSICAL, CONTEMPORARY, COUNTRY, 
            DANCE, DEATH, DELTA, DISCO, DOOM, DRUMANDBASS, DUB, 
            ELECTRONIC, EXPERIMENTAL, FOLK, FUNK, FUSION, GARAGE, 
            GLAM, GLITCH, GOSPEL, GOTH, GRINDCORE, GRUNGE, HARDCORE,
            HARD, HEAVY, HIPHOP, HOUSE, HYPERPOP, IDM, INDUSTRIAL, 
            JAZZ, JUNGLE, KRAUT, LATIN, LOFI, METAL, MIDDLAST, 
            MINIMALIST, MODAL, MODERN, MOTOWN, NEWAGE, NEWWAVE,
            NOISE, OPERA, POP, POSTBOP, PROGRESSIVE, PSYCHEDELIC, 
            PUNK, REGGAE, RNB, ROCK, ROMANTIC, SHOEGAZE, SLUDGE, 
            SOFT, SOUL, SOUTHERN, SPACE, SPEED, STONER, SURF, SWING,
            SYNTH, TECHNO, TEEN, THRASH, TRADITIONAL, TRIPHOP,
            VAPORWAVE, WORLD, UNSPECIFIED
        }

        //==================( ARTIST )==================
        public enum ArtistGenre {
            ROCK,
            METAL,
            PUNK,
            ELECTRONIC,
            POP,
            SOUL,
            HIPHOP,
            JAZZ,
            BLUES,
            FOLK,
            COUNTRY,
            CLASSICAL,
            WORLD,
            UNSPECIFIED
        }
    }
}
