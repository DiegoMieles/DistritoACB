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
        public string user = "448911b8-9ad0-4da0-9996-14ff11174d27";
        [TextArea][SerializeField] [Tooltip("accessToken para la autenticacion del usuario")]
        public string accessToken= "eyJraWQiOiJtc2pNc2NYdk0rWjA4UXZVa3VTRFZiWGRFYmZBd0lRSWVqeEZtOGE1NEdnPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI0NDg5MTFiOC05YWQwLTRkYTAtOTk5Ni0xNGZmMTExNzRkMjciLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9GWm1aRnpKR0wiLCJjbGllbnRfaWQiOiIzaDNkN25qZ3F1aHJqNWhqaTB0b25rZG5rayIsIm9yaWdpbl9qdGkiOiIwN2JkMjI4MS02NDNjLTQ5MTQtYTUzMS0wZGZhNWI0MjE4NGYiLCJldmVudF9pZCI6ImI5MTE4MWMyLWY4YWMtNDhlNy04YmQ0LTk3ZTUxMzYzYzRjNCIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NjMwMTA5MjMsImV4cCI6MTY2MzAxNDUyMywiaWF0IjoxNjYzMDEwOTIzLCJqdGkiOiIxMTNmYzJjZi00Njk4LTQ2ZTktODQ2Yy0yNmJkNGNlNDMyZWUiLCJ1c2VybmFtZSI6IjQ0ODkxMWI4LTlhZDAtNGRhMC05OTk2LTE0ZmYxMTE3NGQyNyJ9.DPjiai8HhnCj-jctA71No3Icq291IEGRAZ7HgjnB-VhWlT-U90Tbp_p4H6FGVw7ci-1nsF5EzgzB24xUYdjoNw2EbFU5EdysFculMA72bdHh4Tma4NlPBtALOVu7u_CaKf0P4hutgPricyS93aw6xJVyaFqBfHjVwKEIXIU-cWI2lDXBW19QnHMddhXHt8EL2oPkDYcfvZSVOT1kIHH_wxCCM-p2SPWnNO7jQbWUpFMSBHjP4xJ-BffwhIoDOzSFrt_JB5YOFZ2mZxlH3TVEHmnNKDDb1Qa4K_kaiqxmWf4sqch0DH0i4-aNWpi3ZRiur5CJhrQszig8F1WTonR0Rg";
        [TextArea]
        [SerializeField]
        [Tooltip("refreshToken para la autenticacion del usuario")]
        public string refreshToken = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAifQ.fNJG5djU6klhNWwsUiJ5H_ykTqTD09ac1XAjlAlM40a8yJrdCW3y0_x8z3A16IQmA1ocp0oKmVKGJ0HXspyZ1ednAUDIwtdwCUINPBbKODVwMWvxJwkZ_solJSF0Gu6titdQACOiHFMAaPquMePqCjo0hAvcLta_AZJZBbJaDN6OK_mHBbPBFn0qdcy2Gp7Y22Xf4pTeeT4srwozn9iwDlU-9ikXoDukb_VuPifWXiocHAndv9vDLLUY1iV6abhgs7aV7ruem32AXW17ZCMXAMq_m7_Qt-yd079s1ekG0oeaVIqfDLuTu4okEFuIwDv9gJAt_LjTMER4KOJ1LF0TzQ.ZuY-Ic9EPoy12zJ-.Rnf4cElyFSSVanh2GKyMDsjTDmvuFktcAfxKgx8GtSeeZ5ZQnmWTGA4WifzW4IEfRX9BDcoFwld_Qm2kWgF7PH2ryMnQ20V0ER8PzukyqKc-hFu-2J7VdwYyth6aKpCojrooHwtW-1RsHCOlXxgJYxqEWRDTe5NlJ7tHFIbYv7iZDgusDygdPrlpLow9oNHnHfI2CuU8zj9yhrfqpOSmag1mnkiywIIQHQHm58IXkB3neFy43VnNqv7ftT9Lhh5POSVPcbGCothe-xVsTDC48YzYYk8pNesStMZju3q5n_dd5x6mUtmHEcNQ0u_5fqxEY53MsPMQ5oe10rSmPApUXjibPGCTX-ChjqbRZpKcwQoCJhX1aNKJ_Jdj6b846RmR_WmZMV7mgUmp7nfgx_XtRSONIwtc_1-ah_iLpYiQI67N1O0zaDbaZoa5rv4YBwLNu4qfG6Z3nOAJJN-Ff_ybj5vTw4BzdQNtUmm0g5VBzhc_JItlMCh_sjHBSz3OesP19xc-V56ci6VMA6TFQA079hmRw4DlZMSzHYqFMhhlM9hBJE9_Oe7EhM9dUnACG29O1XsTSqCSWwMPHnEDrC4GtpCRWGC-0szUo6yAbJDScqz4Y59RhGKKyzhETq4J8GOubN8BgINJVv415hBDUjDrcNZQ6BvTD4NNJlIHU4ItXO-AKvdasHv4dIIpGZPTYcRFChcj6ENJvtba2Je7guGzya547OFgWInSHInPjnfffFZjD0dZ5dRlPh_ZeXBQ_hLP_PiPNHvGWg08kTVAL732Asv1h8ybzxO4O0xmNXs3gB9t62riJcj5EhcR_ALvfJSbtNKkVNUgmweRwPxqXG4SbMYMOgo4OW4gszbKkiIUFCqmHoDBph09a_Wg0tTRBiXs0ohTTH8q5JgcZIvkmcybq-6-QWQ7RocM_5kSo4FUPB6C0lW9bAD3T9q4GSzIHKnMK5JvmztK-QHZ3AAaJpme3gQeyhIlFS71FliSIGHFbNWdUyG_BqP3dbG6lRhQRCOTIjZz0mwujlSb3xgg-cw6zPVy0Gs68KpwjRF-hqpK7N1edc0kWkzU8R-Gk8BSk7CEFyD5A1mVrBJP13884W5xABNjoxXEydVUuW9ykeQvYd-eA7KclkwyXLvMPd_PvL6ttmFBhV7mTf1i2BzIJW0mD3NFVuFk1fNmDUN-66fLZcxCKRiAKCgurHMerYLNSjkWanaW-s_zq_AOlEwb_Q1lNWghi99NqQI2xjIMDbxz9GeopTw1ftMx8UGlGqDQYVMIvHR9PZY4lPBec87fEMBV0ZVR7F6mPk3HOsnsaKXmk6t8YgqSp-mvXTMoxw.5UDPNPUdnhqtzwmUohpTfg";
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