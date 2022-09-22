using System;
using UnityEngine;

namespace WebAPI
{
    [Serializable]
    public class UserData
    {
#if !UNITY_EDITOR
        public string user = "null";
        [TextArea]
        public string accessToken= "null";
        [TextArea]
        public string refreshToken = "null";
        #endif
#if UNITY_EDITOR
        [SerializeField] [Tooltip("identificacion del usuario autenticado")]
        public string user = "0cb690a8-cdfb-4bfc-9695-4fb01a23fd38";
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiJtc2pNc2NYdk0rWjA4UXZVa3VTRFZiWGRFYmZBd0lRSWVqeEZtOGE1NEdnPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiIwY2I2OTBhOC1jZGZiLTRiZmMtOTY5NS00ZmIwMWEyM2ZkMzgiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9GWm1aRnpKR0wiLCJjbGllbnRfaWQiOiIzaDNkN25qZ3F1aHJqNWhqaTB0b25rZG5rayIsIm9yaWdpbl9qdGkiOiJmOTUxZTFmNi0yNzk3LTRjMDktOTliMS05NjM5NjFlOTI1YzMiLCJldmVudF9pZCI6IjIyYTE1YWNkLTgzNGUtNGM0Yi1hMjZlLWMxODQ1ZmM4NGJmYSIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjM3NzU5NzMsImV4cCI6MTY2Mzc3OTU3MywiaWF0IjoxNjYzNzc1OTczLCJqdGkiOiJjZTdjOTJhMC00MGFmLTQ3NDUtODk5Ny1mNjM4NzdlMWVhYTEiLCJ1c2VybmFtZSI6IjBjYjY5MGE4LWNkZmItNGJmYy05Njk1LTRmYjAxYTIzZmQzOCJ9.wSQisOMNJg92I1f4oFeVRf3xi6Bn5We6itDHNpP15chgwWjtdFPnfgDg61jDXw2pz45x3u54mH8j8kBlHqvWukp6r9Z7bViWRbTnjoGzUYW3Bz8zniclCVg8ln3lca6e156OlOuQmT-1qsuuGvatEjJ9iNsXSiBupfWWwsE7b1hvWdR9fhyEGdnbKPZyXPeosKKxndClKmWdJ3hRukMZjEjaRpVgi8grhfID0tcDNRuj7odzvTczxd9kxCvW5cQqkezlkhbe6Y74bOP4_f2BpQG0vvbHpad6tR40Fx1IGyfQPl5xJEAmhRjJWoPQ69DG6CVQ8Ws3KrmlMW1XApSxyg";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.QJXvb7w2zQ58NpYcnITlJrpuc2IUDtfz933Iz3r8EgXWh5J8I1gHrM0-sI3NWo32M2T9V9zGy4z7_7q6toXeqRfP4SGShJmXiooXtq1DGu6S31hO5vkRRAHotcJHoIW-RZF2W4nKjWnwcmqygxMu6N2XGqjccOyw9QjbJdGKuxO-BEbL37DJzc49Q6ek9OTMWnDNirLsXKidcqQXnKWTV1BMUuf3KBdw12qzIwV2wmadkM3xKCyEjec6OgLHfD8IRVZou6fIAh9jI6jHNGh8peHccfRKr_5z7ocxHhQBTQri-vPwbfsiAYjJPD_32gOgQiS7IGAvw5DeeqD6fugvvw.P7odYILnM28I6kNQ.tR8GzqD2_RSrLn8ydOh0MQq-PoNljO6DBPsqgSaucp6apPLLmXFXfNJZKnW1jfmJWnEA8lQ6EC5I_9ZFbRqerUaBriR0a4Rww90egtl64f_K9s-FBNu1ZzlYAgmUWXMGCQUFpUYscsIH4eXW-pSQj7-PcwC3A6N3qLkN4F3TamQM6eQdGSMT8tuiW8ZV9tbtRRMROdwPhFDT2w4GTP-Uanxya1WKD6dajaZADtKczjNxDig4Qg_VyvdBmlWY6xqqcEjnevysb8NZ6FRBEPjFRQD38pYkpouCHn16ijeH_aDbsoVWWf9rU6xKmhsvxWLKHx5UKNh7fgSPZRNHXWKnkfOLC7ZTGzslZyITnonjX43deTV8LOnl_8ao7LOEJ1X6_rzw0s6vanN7noa2SRvh--KD928M4TY5p3D4bVwBRXsTvyQzJUxpJspwIuXoPTYUU7EqkkYOcoepvZ-EXA7z5_aW6qvM4J92SqnXUsj1WKjGW9jvdCpQCFBEbJgHJtQqbgphJXSuqAH-khgSurOfzKr7DcDR1ZpsVzH-w3M2h_7F5D4Acr1t2EHC5EEXdaKMeW1v5DVpMKEknGlBheFJTiJ7VlUrKktNvCdL0p0OVhn4vYVtk4e-vpo41twbLYVg5JqkRvCe-rUSQu1_gkMZN-tLb_lvbm5OrftMjam-ymtY62qPSbcjo7aT_IXnBU5OP-1INJJFQA2hZcW3fuedCUa47mVKwlTk2UW8vDebp7xKBtn6Amy6dGrNRwfCRTKaw9ViZ44CRWMZKzcCTcOfZEkKzvImTLj_AXFfRmLbp_QIi-QSDaKU_2f_8jodqky689pRKR9oUJaN7XASCYU5yrXI2PBvjn5AD-Sudkxc9hatXxekz6G_E3OTNC-DrOmNUIf0KKhFJmLXuCNDclOE2Z7agAX6puz10H7lJLuRRJkbrxHhAHx-nQl0suEpAyZcBiHhH9Hxon-u9HiG9Zczt1P3GO06sXvFd5Os-EO_zkhNhFeingSqUEjwu3vAPZXgeuD3Sq3HdROUzfYaj_wezJxbBE0-UTPg_5yX3E6c2so6SLfVPDenH10LCfLeP6gFY58RcgxykT7RkrOoYFuWyaxXkYwkbn6nBgsesGxVtOmc3DsF8DBDg5gfzfWZ8NrEbICfrGj9H3LXLXpTifvgQFrS5il5FEs066qTOHf3WCksPy9szZ4XZ2sSI0ydss40lYrreG9CDWUbGP5b8cdsNDWdomC4ebvGqJAWo2uV0cfYyKrdL9TQi83RPKW_cBFt2k7He-GHUQT3POYToKk4hCF7l5D2KtwqCLTmwA80ubxFHHgtd5DBD-4ijg.f75wYIHaqD0DipY6x6Xl0Q";
#endif

        public UserData()
        {
#if !UNITY_EDITOR
            user = "null";
            accessToken = "null";
            refreshToken =  "null";
#endif
        } 
    }
}