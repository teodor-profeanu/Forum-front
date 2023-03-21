﻿using AwesomeForum.Data.Base.AweForum.Data.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeForum.Models
{
    public class Message_Reaction : IModelBase
    {
        public int id { get; set; }
        public int messageId { get; set; }
        [ForeignKey(nameof(messageId))]
        public TopicMessage topicMessage { get; set; }
        public int reactionId { get; set; }
        [ForeignKey(nameof(reactionId))]
        public Reaction reaction { get; set; }
    }
}
