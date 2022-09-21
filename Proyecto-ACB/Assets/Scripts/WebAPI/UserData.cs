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
        public string accessToken= "eyJraWQiOiIwbWUrcFc0a3lEXC9VaGVlMDZ4RFwvTnM3UDZPNjVuZjZRb2tRakpFdTVVMXc9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJmNTE0ZGMzYy00ZDA5LTRhNDctODA3OS01ZGZkZDAxMzEyOTQiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMV9ZTk5QczhhWFYiLCJjbGllbnRfaWQiOiIzZzQzNHMwOGw2bDBibnU0dGl1NHRkdm9kMSIsIm9yaWdpbl9qdGkiOiI2MWJmNzYwNS1kNTkyLTQ0MDgtODgyMS0xOWM3NDc3MmUyZTEiLCJldmVudF9pZCI6ImMwMjZhYTI3LTY5NzgtNDgzNi1iMTdhLTU0Y2RkNzVkZmM0ZCIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4iLCJhdXRoX3RpbWUiOjE2NTk2Mjc5ODAsImV4cCI6MTY2MzY4Njk3OCwiaWF0IjoxNjYzNjgzMzc4LCJqdGkiOiI0ZmQ0MzMzOC02M2JkLTRkODItYjUyMS1mM2QzM2IwM2IzMjEiLCJ1c2VybmFtZSI6ImY1MTRkYzNjLTRkMDktNGE0Ny04MDc5LTVkZmRkMDEzMTI5NCJ9.F3v6DOvRhS2Lzv4e4KOiZRvcoVyEQ2Ik_D9kO_Cx5MBVX6szYcptJOB3u7jdi7hgaN3BP9kxXdRuzEDyqKTHqx4DnJCzAGOJRgK1_4iG8D4E3YYI4X_1H00giw812PED5MO8vJW2vfmefsyfygj5U09zgcx05wv4bzAa_AuS_BcLr9NkeEwWZSVDvGUYScGTFD5FQxZP7aMc55RYsKiBrK7LUWmKSvFQqnv1xOPzBD2F6NHNZy4dvTejtbC2BpjyiPIZBisf8K5-m5mEUwhpvc-BeuxB0EY85ttmanngdqG7Z7pRR6z4YqN8SY4wawfAT0cL9EvjOLMjZFe5IoCRYw";
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