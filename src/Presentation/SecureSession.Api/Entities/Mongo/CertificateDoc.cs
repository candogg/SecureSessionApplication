using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SecureSession.Api.Entities.Mongo
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class CertificateDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonElement("PublicKey")]
        [BsonRepresentation(BsonType.String)]
        public string PublicKey { get; set; } = null!;

        [BsonElement("PrivateKey")]
        [BsonRepresentation(BsonType.String)]
        public string PrivateKey { get; set; } = null!;

        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("ValidUntil")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ValidUntil { get; set; }
    }
}
