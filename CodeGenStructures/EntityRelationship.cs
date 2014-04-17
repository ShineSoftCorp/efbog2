namespace voidsoft.efbog
{
	public enum RelationshipType
	{
		Parent,
		Child
	}

	public struct EntityRelationship
	{
		public RelationshipType AssociationType;

		public string RelatedEntityName;

		public string RelatedEntityPrimaryKey;

		public EntityRelationship(RelationshipType associationType, string relatedEntityName, string relatedEntityPrimaryKey)
		{
			AssociationType = associationType;
			RelatedEntityName = relatedEntityName;
			RelatedEntityPrimaryKey = relatedEntityPrimaryKey;
		}
	}
}