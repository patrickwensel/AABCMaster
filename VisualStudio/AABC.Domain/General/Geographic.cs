using System.Collections.Generic;

namespace AABC.Domain.General
{


    public class ZipCode
    {

        public string Zip{ get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }

    }


    public class State
    {

        public string Code { get; set; }
        public string Name { get; set; }

        public State() {

        }

        public State(string code, string name) {
            this.Code = code;
            this.Name = name;
        }
        
        public static List<State> GetStates() {

            List<State> states = new List<State>();

            states.Add(new State("AL", "Alabama"));
            states.Add(new State("AK", "Alaska"));
            states.Add(new State("AZ", "Arizona"));
            states.Add(new State("AR", "Arkansas"));
            states.Add(new State("CA", "California"));
            states.Add(new State("CO", "Colorado"));
            states.Add(new State("CT", "Connecticut"));
            states.Add(new State("DE", "Delaware"));
            states.Add(new State("DC", "District Of Columbia"));
            states.Add(new State("FL", "Florida"));
            states.Add(new State("GA", "Georgia"));
            states.Add(new State("HI", "Hawaii"));
            states.Add(new State("ID", "Idaho"));
            states.Add(new State("IL", "Illinois"));
            states.Add(new State("IN", "Indiana"));
            states.Add(new State("IA", "Iowa"));
            states.Add(new State("KS", "Kansas"));
            states.Add(new State("KY", "Kentucky"));
            states.Add(new State("LA", "Louisiana"));
            states.Add(new State("ME", "Maine"));
            states.Add(new State("MD", "Maryland"));
            states.Add(new State("MA", "Massachusetts"));
            states.Add(new State("MI", "Michigan"));
            states.Add(new State("MN", "Minnesota"));
            states.Add(new State("MS", "Mississippi"));
            states.Add(new State("MO", "Missouri"));
            states.Add(new State("MT", "Montana"));
            states.Add(new State("NE", "Nebraska"));
            states.Add(new State("NV", "Nevada"));
            states.Add(new State("NH", "New Hampshire"));
            states.Add(new State("NJ", "New Jersey"));
            states.Add(new State("NM", "New Mexico"));
            states.Add(new State("NY", "New York"));
            states.Add(new State("NC", "North Carolina"));
            states.Add(new State("ND", "North Dakota"));
            states.Add(new State("OH", "Ohio"));
            states.Add(new State("OK", "Oklahoma"));
            states.Add(new State("OR", "Oregon"));
            states.Add(new State("PA", "Pennsylvania"));
            states.Add(new State("RI", "Rhode Island"));
            states.Add(new State("SC", "South Carolina"));
            states.Add(new State("SD", "South Dakota"));
            states.Add(new State("TN", "Tennessee"));
            states.Add(new State("TX", "Texas"));
            states.Add(new State("UT", "Utah"));
            states.Add(new State("VT", "Vermont"));
            states.Add(new State("VA", "Virginia"));
            states.Add(new State("WA", "Washington"));
            states.Add(new State("WV", "West Virginia"));
            states.Add(new State("WI", "Wisconsin"));
            states.Add(new State("WY", "Wyoming"));

            return states;
        }

    }



    public class Country
    {
        public Country() {
            Name = null;
            Alpha2Code = null;
            Alpha3Code = null;
            NumericCode = null;
        }

        public Country(string name, string alpha2Code, string alpha3Code, string numericCode) {
            Name = name;
            Alpha2Code = alpha2Code;
            Alpha3Code = alpha3Code;
            NumericCode = numericCode;
        }

        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public string NumericCode { get; set; }
                
        public static List<Country> GetCountries() {

            List<Country> countries = new List<Country>();

            countries.Add(new Country("Afghanistan", "AF", "AFG", "004"));
            countries.Add(new Country("Aland Islands", "AX", "ALA", "248"));
            countries.Add(new Country("Albania", "AL", "ALB", "008"));
            countries.Add(new Country("Algeria", "DZ", "DZA", "012"));
            countries.Add(new Country("American Samoa", "AS", "ASM", "016"));
            countries.Add(new Country("Andorra", "AD", "AND", "020"));
            countries.Add(new Country("Angola", "AO", "AGO", "024"));
            countries.Add(new Country("Anguilla", "AI", "AIA", "660"));
            countries.Add(new Country("Antarctica", "AQ", "ATA", "010"));
            countries.Add(new Country("Antigua and Barbuda", "AG", "ATG", "028"));
            countries.Add(new Country("Argentina", "AR", "ARG", "032"));
            countries.Add(new Country("Armenia", "AM", "ARM", "051"));
            countries.Add(new Country("Aruba", "AW", "ABW", "533"));
            countries.Add(new Country("Australia", "AU", "AUS", "036"));
            countries.Add(new Country("Austria", "AT", "AUT", "040"));
            countries.Add(new Country("Azerbaijan", "AZ", "AZE", "031"));
            countries.Add(new Country("Bahamas", "BS", "BHS", "044"));
            countries.Add(new Country("Bahrain", "BH", "BHR", "048"));
            countries.Add(new Country("Bangladesh", "BD", "BGD", "050"));
            countries.Add(new Country("Barbados", "BB", "BRB", "052"));
            countries.Add(new Country("Belarus", "BY", "BLR", "112"));
            countries.Add(new Country("Belgium", "BE", "BEL", "056"));
            countries.Add(new Country("Belize", "BZ", "BLZ", "084"));
            countries.Add(new Country("Benin", "BJ", "BEN", "204"));
            countries.Add(new Country("Bermuda", "BM", "BMU", "060"));
            countries.Add(new Country("Bhutan", "BT", "BTN", "064"));
            countries.Add(new Country("Bolivia, Plurinational State of", "BO", "BOL", "068"));
            countries.Add(new Country("Bonaire, Sint Eustatius and Saba", "BQ", "BES", "535"));
            countries.Add(new Country("Bosnia and Herzegovina", "BA", "BIH", "070"));
            countries.Add(new Country("Botswana", "BW", "BWA", "072"));
            countries.Add(new Country("Bouvet Island", "BV", "BVT", "074"));
            countries.Add(new Country("Brazil", "BR", "BRA", "076"));
            countries.Add(new Country("British Indian Ocean Territory", "IO", "IOT", "086"));
            countries.Add(new Country("Brunei Darussalam", "BN", "BRN", "096"));
            countries.Add(new Country("Bulgaria", "BG", "BGR", "100"));
            countries.Add(new Country("Burkina Faso", "BF", "BFA", "854"));
            countries.Add(new Country("Burundi", "BI", "BDI", "108"));
            countries.Add(new Country("Cambodia", "KH", "KHM", "116"));
            countries.Add(new Country("Cameroon", "CM", "CMR", "120"));
            countries.Add(new Country("Canada", "CA", "CAN", "124"));
            countries.Add(new Country("Cape Verde", "CV", "CPV", "132"));
            countries.Add(new Country("Cayman Islands", "KY", "CYM", "136"));
            countries.Add(new Country("Central African Republic", "CF", "CAF", "140"));
            countries.Add(new Country("Chad", "TD", "TCD", "148"));
            countries.Add(new Country("Chile", "CL", "CHL", "152"));
            countries.Add(new Country("China", "CN", "CHN", "156"));
            countries.Add(new Country("Christmas Island", "CX", "CXR", "162"));
            countries.Add(new Country("Cocos (Keeling) Islands", "CC", "CCK", "166"));
            countries.Add(new Country("Colombia", "CO", "COL", "170"));
            countries.Add(new Country("Comoros", "KM", "COM", "174"));
            countries.Add(new Country("Congo", "CG", "COG", "178"));
            countries.Add(new Country("Congo, the Democratic Republic of the", "CD", "COD", "180"));
            countries.Add(new Country("Cook Islands", "CK", "COK", "184"));
            countries.Add(new Country("Costa Rica", "CR", "CRI", "188"));
            countries.Add(new Country("Cote d'Ivoire", "CI", "CIV", "384"));
            countries.Add(new Country("Croatia", "HR", "HRV", "191"));
            countries.Add(new Country("Cuba", "CU", "CUB", "192"));
            countries.Add(new Country("Curacao", "CW", "CUW", "531"));
            countries.Add(new Country("Cyprus", "CY", "CYP", "196"));
            countries.Add(new Country("Czech Republic", "CZ", "CZE", "203"));
            countries.Add(new Country("Denmark", "DK", "DNK", "208"));
            countries.Add(new Country("Djibouti", "DJ", "DJI", "262"));
            countries.Add(new Country("Dominica", "DM", "DMA", "212"));
            countries.Add(new Country("Dominican Republic", "DO", "DOM", "214"));
            countries.Add(new Country("Ecuador", "EC", "ECU", "218"));
            countries.Add(new Country("Egypt", "EG", "EGY", "818"));
            countries.Add(new Country("El Salvador", "SV", "SLV", "222"));
            countries.Add(new Country("Equatorial Guinea", "GQ", "GNQ", "226"));
            countries.Add(new Country("Eritrea", "ER", "ERI", "232"));
            countries.Add(new Country("Estonia", "EE", "EST", "233"));
            countries.Add(new Country("Ethiopia", "ET", "ETH", "231"));
            countries.Add(new Country("Falkland Islands (Malvinas)", "FK", "FLK", "238"));
            countries.Add(new Country("Faroe Islands", "FO", "FRO", "234"));
            countries.Add(new Country("Fiji", "FJ", "FJI", "242"));
            countries.Add(new Country("Finland", "FI", "FIN", "246"));
            countries.Add(new Country("France", "FR", "FRA", "250"));
            countries.Add(new Country("French Guiana", "GF", "GUF", "254"));
            countries.Add(new Country("French Polynesia", "PF", "PYF", "258"));
            countries.Add(new Country("French Southern Territories", "TF", "ATF", "260"));
            countries.Add(new Country("Gabon", "GA", "GAB", "266"));
            countries.Add(new Country("Gambia", "GM", "GMB", "270"));
            countries.Add(new Country("Georgia", "GE", "GEO", "268"));
            countries.Add(new Country("Germany", "DE", "DEU", "276"));
            countries.Add(new Country("Ghana", "GH", "GHA", "288"));
            countries.Add(new Country("Gibraltar", "GI", "GIB", "292"));
            countries.Add(new Country("Greece", "GR", "GRC", "300"));
            countries.Add(new Country("Greenland", "GL", "GRL", "304"));
            countries.Add(new Country("Grenada", "GD", "GRD", "308"));
            countries.Add(new Country("Guadeloupe", "GP", "GLP", "312"));
            countries.Add(new Country("Guam", "GU", "GUM", "316"));
            countries.Add(new Country("Guatemala", "GT", "GTM", "320"));
            countries.Add(new Country("Guernsey", "GG", "GGY", "831"));
            countries.Add(new Country("Guinea", "GN", "GIN", "324"));
            countries.Add(new Country("Guinea-Bissau", "GW", "GNB", "624"));
            countries.Add(new Country("Guyana", "GY", "GUY", "328"));
            countries.Add(new Country("Haiti", "HT", "HTI", "332"));
            countries.Add(new Country("Heard Island and McDonald Islands", "HM", "HMD", "334"));
            countries.Add(new Country("Holy See (Vatican City State)", "VA", "VAT", "336"));
            countries.Add(new Country("Honduras", "HN", "HND", "340"));
            countries.Add(new Country("Hong Kong", "HK", "HKG", "344"));
            countries.Add(new Country("Hungary", "HU", "HUN", "348"));
            countries.Add(new Country("Iceland", "IS", "ISL", "352"));
            countries.Add(new Country("India", "IN", "IND", "356"));
            countries.Add(new Country("Indonesia", "ID", "IDN", "360"));
            countries.Add(new Country("Iran, Islamic Republic of", "IR", "IRN", "364"));
            countries.Add(new Country("Iraq", "IQ", "IRQ", "368"));
            countries.Add(new Country("Ireland", "IE", "IRL", "372"));
            countries.Add(new Country("Isle of Man", "IM", "IMN", "833"));
            countries.Add(new Country("Israel", "IL", "ISR", "376"));
            countries.Add(new Country("Italy", "IT", "ITA", "380"));
            countries.Add(new Country("Jamaica", "JM", "JAM", "388"));
            countries.Add(new Country("Japan", "JP", "JPN", "392"));
            countries.Add(new Country("Jersey", "JE", "JEY", "832"));
            countries.Add(new Country("Jordan", "JO", "JOR", "400"));
            countries.Add(new Country("Kazakhstan", "KZ", "KAZ", "398"));
            countries.Add(new Country("Kenya", "KE", "KEN", "404"));
            countries.Add(new Country("Kiribati", "KI", "KIR", "296"));
            countries.Add(new Country("Korea, Democratic People's Republic of", "KP", "PRK", "408"));
            countries.Add(new Country("Korea, Republic of", "KR", "KOR", "410"));
            countries.Add(new Country("Kuwait", "KW", "KWT", "414"));
            countries.Add(new Country("Kyrgyzstan", "KG", "KGZ", "417"));
            countries.Add(new Country("Lao People's Democratic Republic", "LA", "LAO", "418"));
            countries.Add(new Country("Latvia", "LV", "LVA", "428"));
            countries.Add(new Country("Lebanon", "LB", "LBN", "422"));
            countries.Add(new Country("Lesotho", "LS", "LSO", "426"));
            countries.Add(new Country("Liberia", "LR", "LBR", "430"));
            countries.Add(new Country("Libya", "LY", "LBY", "434"));
            countries.Add(new Country("Liechtenstein", "LI", "LIE", "438"));
            countries.Add(new Country("Lithuania", "LT", "LTU", "440"));
            countries.Add(new Country("Luxembourg", "LU", "LUX", "442"));
            countries.Add(new Country("Macao", "MO", "MAC", "446"));
            countries.Add(new Country("Macedonia, the former Yugoslav Republic of", "MK", "MKD", "807"));
            countries.Add(new Country("Madagascar", "MG", "MDG", "450"));
            countries.Add(new Country("Malawi", "MW", "MWI", "454"));
            countries.Add(new Country("Malaysia", "MY", "MYS", "458"));
            countries.Add(new Country("Maldives", "MV", "MDV", "462"));
            countries.Add(new Country("Mali", "ML", "MLI", "466"));
            countries.Add(new Country("Malta", "MT", "MLT", "470"));
            countries.Add(new Country("Marshall Islands", "MH", "MHL", "584"));
            countries.Add(new Country("Martinique", "MQ", "MTQ", "474"));
            countries.Add(new Country("Mauritania", "MR", "MRT", "478"));
            countries.Add(new Country("Mauritius", "MU", "MUS", "480"));
            countries.Add(new Country("Mayotte", "YT", "MYT", "175"));
            countries.Add(new Country("Mexico", "MX", "MEX", "484"));
            countries.Add(new Country("Micronesia, Federated States of", "FM", "FSM", "583"));
            countries.Add(new Country("Moldova, Republic of", "MD", "MDA", "498"));
            countries.Add(new Country("Monaco", "MC", "MCO", "492"));
            countries.Add(new Country("Mongolia", "MN", "MNG", "496"));
            countries.Add(new Country("Montenegro", "ME", "MNE", "499"));
            countries.Add(new Country("Montserrat", "MS", "MSR", "500"));
            countries.Add(new Country("Morocco", "MA", "MAR", "504"));
            countries.Add(new Country("Mozambique", "MZ", "MOZ", "508"));
            countries.Add(new Country("Myanmar", "MM", "MMR", "104"));
            countries.Add(new Country("Namibia", "NA", "NAM", "516"));
            countries.Add(new Country("Nauru", "NR", "NRU", "520"));
            countries.Add(new Country("Nepal", "NP", "NPL", "524"));
            countries.Add(new Country("Netherlands", "NL", "NLD", "528"));
            countries.Add(new Country("New Caledonia", "NC", "NCL", "540"));
            countries.Add(new Country("New Zealand", "NZ", "NZL", "554"));
            countries.Add(new Country("Nicaragua", "NI", "NIC", "558"));
            countries.Add(new Country("Niger", "NE", "NER", "562"));
            countries.Add(new Country("Nigeria", "NG", "NGA", "566"));
            countries.Add(new Country("Niue", "NU", "NIU", "570"));
            countries.Add(new Country("Norfolk Island", "NF", "NFK", "574"));
            countries.Add(new Country("Northern Mariana Islands", "MP", "MNP", "580"));
            countries.Add(new Country("Norway", "NO", "NOR", "578"));
            countries.Add(new Country("Oman", "OM", "OMN", "512"));
            countries.Add(new Country("Pakistan", "PK", "PAK", "586"));
            countries.Add(new Country("Palau", "PW", "PLW", "585"));
            countries.Add(new Country("Palestine, State of", "PS", "PSE", "275"));
            countries.Add(new Country("Panama", "PA", "PAN", "591"));
            countries.Add(new Country("Papua New Guinea", "PG", "PNG", "598"));
            countries.Add(new Country("Paraguay", "PY", "PRY", "600"));
            countries.Add(new Country("Peru", "PE", "PER", "604"));
            countries.Add(new Country("Philippines", "PH", "PHL", "608"));
            countries.Add(new Country("Pitcairn", "PN", "PCN", "612"));
            countries.Add(new Country("Poland", "PL", "POL", "616"));
            countries.Add(new Country("Portugal", "PT", "PRT", "620"));
            countries.Add(new Country("Puerto Rico", "PR", "PRI", "630"));
            countries.Add(new Country("Qatar", "QA", "QAT", "634"));
            countries.Add(new Country("Reunion", "RE", "REU", "638"));
            countries.Add(new Country("Romania", "RO", "ROU", "642"));
            countries.Add(new Country("Russian Federation", "RU", "RUS", "643"));
            countries.Add(new Country("Rwanda", "RW", "RWA", "646"));
            countries.Add(new Country("Saint BarthÃ©lemy", "BL", "BLM", "652"));
            countries.Add(new Country("Saint Helena, Ascension and Tristan da Cunha", "SH", "SHN", "654"));
            countries.Add(new Country("Saint Kitts and Nevis", "KN", "KNA", "659"));
            countries.Add(new Country("Saint Lucia", "LC", "LCA", "662"));
            countries.Add(new Country("Saint Martin (French part)", "MF", "MAF", "663"));
            countries.Add(new Country("Saint Pierre and Miquelon", "PM", "SPM", "666"));
            countries.Add(new Country("Saint Vincent and the Grenadines", "VC", "VCT", "670"));
            countries.Add(new Country("Samoa", "WS", "WSM", "882"));
            countries.Add(new Country("San Marino", "SM", "SMR", "674"));
            countries.Add(new Country("Sao Tome and Principe", "ST", "STP", "678"));
            countries.Add(new Country("Saudi Arabia", "SA", "SAU", "682"));
            countries.Add(new Country("Senegal", "SN", "SEN", "686"));
            countries.Add(new Country("Serbia", "RS", "SRB", "688"));
            countries.Add(new Country("Seychelles", "SC", "SYC", "690"));
            countries.Add(new Country("Sierra Leone", "SL", "SLE", "694"));
            countries.Add(new Country("Singapore", "SG", "SGP", "702"));
            countries.Add(new Country("Sint Maarten (Dutch part)", "SX", "SXM", "534"));
            countries.Add(new Country("Slovakia", "SK", "SVK", "703"));
            countries.Add(new Country("Slovenia", "SI", "SVN", "705"));
            countries.Add(new Country("Solomon Islands", "SB", "SLB", "090"));
            countries.Add(new Country("Somalia", "SO", "SOM", "706"));
            countries.Add(new Country("South Africa", "ZA", "ZAF", "710"));
            countries.Add(new Country("South Georgia and the South Sandwich Islands", "GS", "SGS", "239"));
            countries.Add(new Country("South Sudan", "SS", "SSD", "728"));
            countries.Add(new Country("Spain", "ES", "ESP", "724"));
            countries.Add(new Country("Sri Lanka", "LK", "LKA", "144"));
            countries.Add(new Country("Sudan", "SD", "SDN", "729"));
            countries.Add(new Country("Suriname", "SR", "SUR", "740"));
            countries.Add(new Country("Svalbard and Jan Mayen", "SJ", "SJM", "744"));
            countries.Add(new Country("Swaziland", "SZ", "SWZ", "748"));
            countries.Add(new Country("Sweden", "SE", "SWE", "752"));
            countries.Add(new Country("Switzerland", "CH", "CHE", "756"));
            countries.Add(new Country("Syrian Arab Republic", "SY", "SYR", "760"));
            countries.Add(new Country("Taiwan, Province of China", "TW", "TWN", "158"));
            countries.Add(new Country("Tajikistan", "TJ", "TJK", "762"));
            countries.Add(new Country("Tanzania, United Republic of", "TZ", "TZA", "834"));
            countries.Add(new Country("Thailand", "TH", "THA", "764"));
            countries.Add(new Country("Timor-Leste", "TL", "TLS", "626"));
            countries.Add(new Country("Togo", "TG", "TGO", "768"));
            countries.Add(new Country("Tokelau", "TK", "TKL", "772"));
            countries.Add(new Country("Tonga", "TO", "TON", "776"));
            countries.Add(new Country("Trinidad and Tobago", "TT", "TTO", "780"));
            countries.Add(new Country("Tunisia", "TN", "TUN", "788"));
            countries.Add(new Country("Turkey", "TR", "TUR", "792"));
            countries.Add(new Country("Turkmenistan", "TM", "TKM", "795"));
            countries.Add(new Country("Turks and Caicos Islands", "TC", "TCA", "796"));
            countries.Add(new Country("Tuvalu", "TV", "TUV", "798"));
            countries.Add(new Country("Uganda", "UG", "UGA", "800"));
            countries.Add(new Country("Ukraine", "UA", "UKR", "804"));
            countries.Add(new Country("United Arab Emirates", "AE", "ARE", "784"));
            countries.Add(new Country("United Kingdom", "GB", "GBR", "826"));
            countries.Add(new Country("United States", "US", "USA", "840"));
            countries.Add(new Country("United States Minor Outlying Islands", "UM", "UMI", "581"));
            countries.Add(new Country("Uruguay", "UY", "URY", "858"));
            countries.Add(new Country("Uzbekistan", "UZ", "UZB", "860"));
            countries.Add(new Country("Vanuatu", "VU", "VUT", "548"));
            countries.Add(new Country("Venezuela, Bolivarian Republic of", "VE", "VEN", "862"));
            countries.Add(new Country("Viet Nam", "VN", "VNM", "704"));
            countries.Add(new Country("Virgin Islands, British", "VG", "VGB", "092"));
            countries.Add(new Country("Virgin Islands, U.S.", "VI", "VIR", "850"));
            countries.Add(new Country("Wallis and Futuna", "WF", "WLF", "876"));
            countries.Add(new Country("Western Sahara", "EH", "ESH", "732"));
            countries.Add(new Country("Yemen", "YE", "YEM", "887"));
            countries.Add(new Country("Zambia", "ZM", "ZMB", "894"));
            countries.Add(new Country("Zimbabwe", "ZW", "ZWE", "716"));

            return countries;

        }

        
    }



}
