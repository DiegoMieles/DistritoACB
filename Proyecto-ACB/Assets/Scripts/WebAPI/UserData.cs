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
        public string user = "81d75f0f-15e4-47cf-8ab9-fccf8ba79009";
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiJtc2pNc2NYdk0rWjA4UXZVa3VTRFZiWGRFYmZBd0lRSWVqeEZtOGE1NEdnPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI4MWQ3NWYwZi0xNWU0LTQ3Y2YtOGFiOS1mY2NmOGJhNzkwMDkiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9GWm1aRnpKR0wiLCJjbGllbnRfaWQiOiIzaDNkN25qZ3F1aHJqNWhqaTB0b25rZG5rayIsIm9yaWdpbl9qdGkiOiI4YTUzYTcxMC1jNjdlLTRmYzUtYjVlMC04NTMyODQ0NzE3YTciLCJldmVudF9pZCI6IjMwYjI1OTg5LTk4Y2UtNDIxNC05NjAwLWI4OWJhZGFmYTkzYyIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjMwMjQ5NTMsImV4cCI6MTY2MzAyODU1MywiaWF0IjoxNjYzMDI0OTUzLCJqdGkiOiI5MjQ4NDVhNi03NTk0LTQxMTUtOGYzZC1jZWM4OGZhZDhjMzEiLCJ1c2VybmFtZSI6IjgxZDc1ZjBmLTE1ZTQtNDdjZi04YWI5LWZjY2Y4YmE3OTAwOSJ9.lpsx4xMmBUu6Ur59_ZdQzLdJouW0imud64rmUwl06cW5314MXkNKkdEERhlwiEneorZVhlOzLhYBMQHHWLxUO4WKeZM9nbHrNgufMpUukgEVdBPeFLEBpw4V6U2Cp2qFUH7RpzaWGHGRbO43m4WqcvzC5U6N7oXD763NCr9X7kaz4cQlqWjSg2Cgqs6Wz28WwddaYTZlz8emmFTecoIkjIUjr5sSsRYCR1hAAXElLOS-TeA-uZsQLF1aLYXntgCZ_pakvLrq6I3PwejPuCVyr09d3HUV7oXd-ZUSf2faeq8f-14Rvn0SWYwIp1cr61JwDl0fAUd33_DDxMrqQcz_9Q";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.Knxdcp2Ke97bfuUSzoYufJP5pefd8sQH7hTBkcULLis2oOCFpvUhgYnoKR1gjrIQ3as9OEER1S005yCrV-SaR_gqPotI1hjYECwSoBcB1ZcyTcuAg60P9Tkf2T-G0bsSvEk3En0FSGqKiurh4E8v7Erj-M8RQ9EHVfbqLv-nz0ebnMYFSWTz0bHfi9LWyEnyUUTWZo5khbHxXaXYstcbtgvjNaE9WgQ26WuEYyA8K9nv_EHb2o0KHMzOllFZbEFF0F7L-rmP5Dk5xrvz0nWXMK2rUhs8hXJe-ArLfS8bAXQto8nonewNNy2KqBAiDdUEcG98cciGFTqzX9Tx-k5QRg.HG61Y0EEF7v7YdB5.qUGNABoD6pWyi3Gwhxb6Tzaz7YSEA7k2fahowkmCowTFgHl_NDGXjY5NwPgb34SPZlyggZxkqzCuJdH4p6RgubQblLYF-7w79YejyGnaKI8v1SpyEjnkAZNpLIFignVuQ5qcIDV7P9orJwKZdqxednr9sGpr5hepQwF2TmcLOFa8tsd72m-FtR9I68Lm_kJAnBKndZqZe2rNaREUF_P2rhQ5oLaW6Q988MsnwtzwUTQvB47bf0nbG0jm9j3fv2fW-V3mIkbTABW0WAyA597UB0Qi10SX64CqtguKHP3nG0b1ULm2wzC5GiM3t23MePJI9XmEwS5N3Rs2TUGSae2zpBYQK8osrRTJrsYCm5i5PVKjJazuulXjhfnJwv55OVn26nF0fbdXVWes6rACAVa-igZNJydK0XesOBZqAiZcseYparB2VkcTXJhn87hTczV98lDyw2RzbHEwJLLjMBPPaOn65mhsjT5WW2z1Txn3Y5hqKjPor3qutPKxhm8qmuuu4YSjwdsA9r5-0Nt4vqX6_VTaz_Mg39B4-RIBGL6TxnmHj4JDQ9YcAPhIE8OGZ6sf2Y2G-NCjKjb8sby9JM_pTud2QgPCg5EHKaBvEI25Dk7ORAT4sqkXkqgdImOryaNe1DgPudYJgb88ijcXA2TK7PXgea4I5bHXOmxd7-R-lOzpkElOebdmDqcAqbC6ROoB9Ec3m9FE-oaJSHceQ9mGbi6hBG6HQt0c0BKIGb7pY-bicMW1OsIs36lJcXzw-fzDd9LdPIzBT6AFlvG1eF3euREkA0fe-3UDwqcCvvMtkpy-rj3QT87Xc9yk5z6gQO6vhjW4ahDzVmzs7dnBNNr2qeZ2Oh_iTVMyR9HoMmA0fTq-ra9JupNLtAqCipcSLnLmN1fXDmQzg7fYCPSeq3mrDiaP3Cmt6-vW-RziPBkAYG3oM-jmKkdopxwkkkP8ynBfnQq9ggkgr0NI2e0c4PtwHpEycT9hZP0SADLnQbIbnqWcbru8dU6owHANDou12dSNouRmE3oe2Fc8vls95g9jorwrGwJOOAvh9AfBqHZC1v2lCCPBBuqMuHIQ2r6cPFwTm5Cu0_b9LQbyHtjx-1biPSDtqvxSyhEBpb7mixAZUXu_HkhSl_ox2-6lsz1nx3-xl7WIIP6H5BGaYDUwJcEyjpkOn4mlTBtNhU6J6VZP56h_fCtL4VYoI4OGOEeF1tOj2bv1Ue3PaH4jwSCQbUp4AncbbWkvhKe5O1q8BtkeS2b25HEy8fdPmLQipxISuyIhTPq3Pr87rAZQAXafuq8t1LQqJ8dzAfvYDb0oNIcp65Dgb6TqJyPGmhKeMw.pDPBmkDlPiPfQJEgGhgi0w";
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