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
        public string accessToken= "eyJraWQiOiIwbWUrcFc0a3lEXC9VaGVlMDZ4RFwvTnM3UDZPNjVuZjZRb2tRakpFdTVVMXc9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJiNTA0YTQwYy1lZWJjLTQwMWUtOTM2YS1jMWE5MDY3M2QyNTciLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9ZTk5QczhhWFYiLCJjbGllbnRfaWQiOiIzZzQzNHMwOGw2bDBibnU0dGl1NHRkdm9kMSIsIm9yaWdpbl9qdGkiOiIxMWNiZDk4Yy0xYmM5LTQxM2UtYjcwNy03NDFjNzU4ZWRiZjQiLCJldmVudF9pZCI6ImRhMjBiNTAzLTI1MDYtNDNhNi1iN2MyLWVkMmVkYTRjNWJlOSIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjQyOTczOTgsImV4cCI6MTY2NDMwMDk5OCwiaWF0IjoxNjY0Mjk3Mzk4LCJqdGkiOiI2NzhkMDk3Zi00NDA1LTQxY2ItOTg0Yi0yOTY2Y2MzY2NhNzUiLCJ1c2VybmFtZSI6ImI1MDRhNDBjLWVlYmMtNDAxZS05MzZhLWMxYTkwNjczZDI1NyJ9.A-9Zn59yzVQqJTLIogMNLnYs-PDtXz_hSaRFlYi1_TDe6hBPIte7mw82NjSkjS7YbStgaOXOTz9N2QHfLRkX7rzCXVi5ejsZLF-dMntFbMyQLcafRS16z30RNFooF-togZLTqLGsRIX_rOVE-8T0lQfsJeJ6m7cuZm-HNB2nFsmvtp32ZenN_iEtoiCupfHUeJ-_poxPStxixL18BPNdXVkPXRVbi88tCMn4GYGi15SnUIvqxxSed05AZjIUbpDjYVULBCShd-SinesAKvF2mnFS_igFF1Gt0Pfu9lddrdVR-h4NbxsLAPPP5eo1QFj-ezkcPdRyZPNPvUTsKB8sIA";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.JthORaLZEVc0okocYGTphZ6wusAQ3X5yu9mGmGrVpO8TMQrjHCsRpCQHRQPqrtiIDOcfkZ0XcL10UikbFOAzSjnd9_DoW5e5vlbPxas0pe3_GYfbjlu5hBBvSozqUEua8bDcBe0IrO8OZj5QIyIZQ_QwdC0Wn_ZCXfTw08_MhOoqPE0q5le4b3x7bf8nLu_b9YpWajEtyL1AjA3H3M3_AP4Ivc2wrdBhUhPnAd9YgIG6bOBDjNZ0kDx9CpQAK1DgQ_zU7aV7iwZaZUCMVGiYQPd3JKhwa2UBkC32ts6QorqXHivUUJUGfBdrCd_QfPpRhDlix9bVsgPbe8SRidoDfA.GOHOBAvYFc2tpwbk.JBmetDskHxsF2JADZGfogdmr-7_1GI1JEwhPLUXrL2itDumbc5MbDEnKrqVTJeeFz9wl3Z0bkUOcsfkjYNzdPxMRUopUpfxZrimRle44imjMS5F54AMyjlXBIliHOagQLYXbFGGgtwG-uBkuRVq_p6t03YKvSy5DjGGZcLT_AHNiqYiLG83XGYEO5N3SGpyTuGV_WQZdRiLQUKAjEwPyakrhax4cUBC7U077UGT6TyXZhJ-yuhGDhi5LtRoooecSCOpFClF82099jXZylmDCTRajPvpduAKIu8pJgLiigswO43SJlVf2t9Dj1Q-cU7X6Vh5mtRVb3iRmrHUD4PY5wYv4413dtSDEDRsHYj8QBsC9q6SXn7GOkVbOhg2OY8NpcBNDzvSPYboU7FyFG1ek29BaWSQSYfLSBqsfHcbXaQQ2e_Pya1wIBRRi9NyC8QJTs9O37bNWKxwXsf6wdt6e20UweLb325Gp0EXJ1Odgb4UyzCC-IJ8iHQ5qtz7XHukXpiCr0bVgdYTKVxkWWWpUIImtW6KgqTukMi0oWzDz1rfriVmhWUA4b2OLT0yMiTeED-J9CJuv2JvGbSF2G6wGt-DIeKd3r1Oh4TL4objhQcOviT0jCwsUxVYWacDE4zHJBVBm7MPRxSu1C-R1O-K8-g6hSo43xhrUpRwT28pImOq8FGVAl8dPNsL8xVcOPM4rCyIOednqiC8N1GXX29VlN0Uh-WBsXP7SaC9QJF8vcM0AWKnjMW62byLEqHIW3AMf_FmfPAXx3G5MaQuZvuW5uA779gVsBHosYJcxfJ-Ly9FKixfqgAYD0tHsgd6rYD0dmQc7Qk6Jux_BNn__YA3mAUIXGacG36d9KA6CR5_EAHzQNqvNLDNQD_uley3ZwOGZydPhakn7UAqUYZgneQ7BmgpHdkXxBDqS_P22Kxubzng4QQN9iB-CZL6K7-SLtLDuZbuZbQ6A02SZBXMlNdWOHildYeug65_4gr1qiwWIDo3-_iWaBttFSAbMKTaRXt5D8IDgpOz5DL6FvfghJLe5ODIsnC-FR8Xmv3IuT2m2Zk6yChmLLRwG58Q8VPk5VG5y4q8172x6mf_5XnHKP2lVrCzagd7uV9-cF4dx3uMd9Do0lRwoWxJWBr5Nd0Ke59PAAOh2NUC-4I1_1CpDV6O1veqsFKT0XRy7FDcv-9PQK3rCuXNKnP_z7ZcUuabAlQtO15QPIoiK1fH0G8ksD9N3lNCxq2cOqtxynN9anDHmN8rAL0eWYVJQUP79TuORUOIaWclNS2MjjQFGjjB6lKdDRZohw3WDHXqCdd9jN3pLUEqwqv5mto7f5yLHNnFt_Q.IujkqO8UNbeqMjP62nDTRQ";
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