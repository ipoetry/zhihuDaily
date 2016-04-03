using System.Runtime.Serialization;

namespace zhihuDaily.Model
{
    [DataContract]
    public class StoryExtra:NotificationObject
    {
        [DataMember(Name = "post_reasons")]
        public int PostReasons { get; set; }

        [DataMember(Name = "favorite")]
        private bool favorite;
        public bool Favorite {
            get { return favorite; }
            set {
                favorite = value;
                RaisePropertyChanged(()=> Favorite);
            }
        }

        [DataMember(Name = "normal_comments")]
        public int NormalComments { get; set; }
        private int longComments;
        [DataMember(Name = "long_comments")]
        public int LongComments {
            get { return longComments; }
            set {
                longComments = value;
                RaisePropertyChanged(()=> LongComments);
            }
        }

        [DataMember(Name = "popularity")]
        public int Popularity { get; set; }
        private int shortComments;
        [DataMember(Name = "short_comments")]
        public int ShortComments {
            get { return shortComments; }
            set {
                shortComments = value;
                RaisePropertyChanged(()=>ShortComments);
            }
        }
        private int comments;
        [DataMember(Name = "comments")]
        public int Comments {
            get { return comments; }
            set {
                comments = value;
                RaisePropertyChanged(()=>Comments);
            }
        }

        [DataMember(Name = "vote_status")]
        private int voteStatus;
        public int VoteStatus {
            get { return voteStatus; }
            set {
                voteStatus = value;
                RaisePropertyChanged(()=> VoteStatus);
            }
        }

        //{"post_reasons":1,"popularity":64,"favorite":false,"normal_comments":12,"comments":13,"short_comments":10,"vote_status":0,"long_comments":3}

    }
}
