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
        public string user = "b504a40c-eebc-401e-936a-c1a90673d257";
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiIwbWUrcFc0a3lEXC9VaGVlMDZ4RFwvTnM3UDZPNjVuZjZRb2tRakpFdTVVMXc9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJiNTA0YTQwYy1lZWJjLTQwMWUtOTM2YS1jMWE5MDY3M2QyNTciLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9ZTk5QczhhWFYiLCJjbGllbnRfaWQiOiIzZzQzNHMwOGw2bDBibnU0dGl1NHRkdm9kMSIsIm9yaWdpbl9qdGkiOiI2NWE0MzFiNS0yNTI2LTQ2YTItOWZmNC1kN2VjZjVhNDY2OGEiLCJldmVudF9pZCI6IjU5M2VhNjZhLWE1YWYtNGI2Yi04ZGRlLWQ1ODBlNDM1ZTViMSIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjY4ODE0MTYsImV4cCI6MTY2Njg4NTAxNiwiaWF0IjoxNjY2ODgxNDE2LCJqdGkiOiI5YjNiOWFkZC04YmJiLTRjZDktYTVkZS1jYjFjM2JkMDExMWEiLCJ1c2VybmFtZSI6ImI1MDRhNDBjLWVlYmMtNDAxZS05MzZhLWMxYTkwNjczZDI1NyJ9.FBAhA0iHp6XADoe8KbTigXbu5dFiKJgQFsUOmJzxvkAowgasobQYxR-ewdjy29zIB-7p0gVDbo5lK6Y5n5tonIBx6KB3JnhNRR-rp09WpmPMfVrJ4RFa4cf-oGG1JyMwkbdDWL3H7f5MOZ_yoDJ9BEMmGYXRuky-R6IEumoFDDrL0ATmygceFqXUXbwx5n6e_6seLAG3TtwInKXJmSht--X96qWNNRmKczDg4x0NOFKHVbObMFmKPMvaDuAlRWDryyA0q-bY-n2qH-FvU3rsZRFfbKuMBs_fNh29Y7NDTionaCP3UJwstrq3LYTgMdCMf1Gkcs65Txlzd147kZ-exw";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.tmy2zj-9sYacav2fE7R1x4YMTdb-6INRdpXJLXX_PylUbPE0CW8G07rDVAdcKTVQBbeqNZCj_eYxmMkKi3gKutfy1jJE-bQneA8lp94RK1y32msYqEeaVPxAOPpTw43KzdTXPiTPNpDDS_D-4yw6i2QmPlo9GzEljlPdnCkZfzZAvx4p7S_FagflNG2-RUIcTZifW4bYEnlatJbv57VRVIAEpC2QdYcdgL1K76BqnIB4xgkfXJNLz2IShN44J_Ke00AIyZ98oNpMJWvsrVoZJnIw1DYavc6A-vPKXZSKOhZHGC7yo8pQ_Jl610BdoHSRY3kZ6gK2LPmteUiD6Ne0kA.978w9ybgFhURvl15.OM1Nni0VXAFV_wPhKqZwmkXuUlvuJn_NsDACWEy3T4k_mHiQnV0dSva5gkLHGAvL7iCNzLthwXt5z1ehjMKPtVslH72jmpHPQMi7AWhf_BPtm6PRxDUOMPivMY7iQtA_F6M02znipJFMjtDECzfvz47Z4kkQVhuCgdjc8FPfLLkOeMW99qhWgp1aFmNZDFV-DblaRcZtU5qW8pe_qGdGiU7vPxBfZQRX3PAtEtIfhAIXRZsdIUNDkFrCfMk7HUTBsk0v0HmJyPq6sls_rpu9LlqgE6q-mzYfvDJIP4OfuOtNW3KJIwtzyyEyV-mhgmyS-XIznmj1Uucily1yH0I9sp6qERG8lw9LL7VUIb8KMonMuvlIzf_YV_5qdQO_QONXD8fLMt_zmrwmxn7PiLqnXZZWxuur9zMAFCZk5tI9IHqcIC5YxQnSoqBfQw5fNxMPXw6qU1H4C2i1bxmx6XoZCDuQTYxqDwKQ_hBcsrQltDhOWJkeCpmBumPdA5FnX9p4PPyFazCkotWTBxT1DeAFkv4T0R3I5r6obna1aKqTMhO4sizTn2PrBGxjwFj85-5E4MZ9VflBJNw5NWtdjgAVij06_fksgWZU0lO-X_NnVGKbg_6Th-FK0ii0Dm_siHceZVsITOxFsj82VyUdzDPPfBZ8c80cbXa4EvXJ57hNe6AkYn8HlDdabKmh3zIqpDBZoPv9wFtcodVPi6BhLVJHfNQIoLurVIo7pIHBmbuo1rbTHcPp2wy-kR-fx16kUOFcNCky7TzoyVvPzjXtRBOV4aNLvmU3DB9hHwcPvdcAU4JD6V3G12qvIvsqvpu-NxXkKg1F8LN-VdNZMzcG2nba1FvXygMu6_-TMsKi-n84JhjoMOIJI3arjLmf4AQIjhQRh_P11vV5T1dYFa3-GlXajFByi5scBU-Da6QehdjoN-B4-UDWryOKv-_DLANvKbK1uTRyKZqW7p00Ccro9CKjBKOPaL3MMAMnz6uYJkYQg-gHudq78CDbrBVoKJvkCINGpM_CX8DG4XOhRGzggA331Ke6f2xdBxZBZgP79WO7x5O3xnachQECHF5BHSckaKNlSfnZw8RW0DVrvVd21wlXoXoDTP7XRuAYs5GfSY_349DMH5X4kFkvcMIfJJeo7aMYj_IPxvqIYfjYbcpNpY5x2kSHNqHqqlm433DH4dhx3kfClbheSVaoHZ2mMr2KjWncePeBrU3kai29ImATook7LvfJCNB5NjLAllwpR2DlMa-HemC1Euz7Gs8V0S9Bov9p73uzJ5Q4sItOw_KTC2XGDZIpU1YPod3NicC6SzNOLl633l9yYzUhSKxzajJmQw.N0YwOkAjUCDZR5HKVwbmvw";
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