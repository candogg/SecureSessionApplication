using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SecureSession.Api.Entities.Mongo
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class SessionDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonElement("SessionCode")]
        [BsonRepresentation(BsonType.String)]
        public string SessionCode { get; set; } = null!;

        [BsonElement("AesKey")]
        [BsonRepresentation(BsonType.String)]
        public string AesKey { get; set; } = null!;

        [BsonElement("AesIv")]
        [BsonRepresentation(BsonType.String)]
        public string AesIv { get; set; } = null!;

        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("ValidUntil")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ValidUntil { get; set; }
    }
}
