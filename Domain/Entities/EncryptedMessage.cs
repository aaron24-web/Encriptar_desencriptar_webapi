// Domain/Entities/EncryptedMessage.cs
using System;

namespace ENCRYPT.Domain.Entities
{
    public class EncryptedMessage
    {
        public long Id { get; set; }
        public required string PlainText { get; set; }
        public required string EncryptedText { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}