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
            VARIOUS,
            UNSPECIFIED
        }

        public enum SubFormat {
            STANDARD, // Common main format
            THREE_INCH,
            SEVEN_INCH,
            TEN_INCH,
            TWELVE_INCH,
            ACETATE,
            FLEXI,
            LATHE,
            PICTURE_DISC,
            BLURAY,
            CD_R,
            DVD,
            DVD_R,
            LASERDISC,
            EIGHT_TRACK,
            REEL_TO_REEL,
            CUSTOM,
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
            UNOFFICIAL,
            UNSPECIFIED
        }

        public enum AlbumGenre {
            ABSTRACT, ACID, ACOUSTIC, ADULT, AFROBEAT, ALTERNATIVE, AMBIENT, AMERICANA, ARABIC, ART, ASIA, AVANTGARDE, BAROQUE, BASS, BEATBOX, BEBOP,
            BIGBAND, BLACK, BLUEGRASS, BLUES, BOOGIE, BOSSANOVA, BRAZILIAN, BREAKBEAT, CAJUN, CARIBBEAN, CELTIC, CHAMBER, CHICAGO, CHILLWAVE,
            CHIPTUNE, CHORAL, CHRISTMAS, CLASH, CLASSICAL, COMEDY, CONCRETE, CONTEMPORARY, CONTINENTAL, COOL, COUNTRY, CREOLE, CROSSOVER, CUBAN,
            DANCE, DARK, DEATH, DEEP, DELTA, DETROIT, DISCO, DIXIELAND, DOOM, DOWNTEMPO, DREAM, DRILL, DRONE, DRUMANDBASS, DUB, DUBSTEP, EARLY,
            EASYLISTENING, EFFECTS, ELECTRONIC, ENSEMBLE, ETHNO, EUROPEAN, EXPERIMENTAL, FOLK, FREAK, FREE, FREESTYLE, FRENCH, FUNK, FUSION,
            GANGSTA, GARAGE, GLAM, GLITCH, GOA, GOSPEL, GOTHIC, GREGORIAN, GRIME, GRINDCORE, GRUNGE, GYPSY, HARD, HARDCORE, HARDBOP, HAUNTOLOGY,
            HEAVY, HIPHOP, HOUSE, HYPERPOP, HYPNAGOGIC, IDM, IMPRESSIONIST, INDIAN, INDIE, INDUSTRIAL, INSTRUCTIONAL, INSTRUMENTAL, ITALO, JAZZ,
            JPOP, JUNGLE, KIDS, KITSCH, KLEZMER, KPOP, KRAUT, LATIN, LEFTFIELD, LOFI, LOUNGE, MATH, MEDIEVAL, METAL, MIDDLEAST, MINIMALIST, MODAL, 
            MODERN, MOTOWN, MUSICAL, NEO, NEWAGE, NEWWAVE, NOIR, NOISE, NU, OPERA, ORCHESTRAL, OUTSIDER, POP, POST, POSTMODERN, POWER, PROGRESSIVE,
            PSYCHEDELIC, PUNK, RAGTIME, RAP, REGGAE, RELIGIOUS, RENAISSANCE, RNB, ROCK, ROCKABILLY, ROMANTIC, SAMBA, SHOEGAZE, SKA, SLUDGE, SMOOTH, 
            SOFT, SOUL, SOUTHERN, SPACE, SPEED, SPIRITUAL, SPOKEN, STONER, SURF, SWAMP, SWING, SYMPHONIC, SYNTH, TECHNO, TEEN, THEATRE, THRASH, 
            TRADITIONAL, TRANCE, TRAP, TRIBAL, TRIPHOP, TURKISH, TURNTABLISM, UNDERGROUND, URBAN, VAPORWAVE, VARIOUS, VOCAL, WORLD, UNSPECIFIED
        }

        //==================( ARTIST )==================
        public enum ArtistGenre {
            BLUES,
            CLASSICAL,
            COUNTRY,
            ELECTRONIC,
            FOLK,
            HIPHOP,
            JAZZ,
            METAL,
            POP,
            PUNK,
            ROCK,
            SOUL,
            WORLD,
            UNSPECIFIED
        }
    }
}
