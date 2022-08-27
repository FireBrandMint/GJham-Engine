
public interface CollisionAntenna
{
    void OnCollision(CollisionAntenna colSubject, ref CollisionResult colInfo);

    void ResolveOverlap(CollisionAntenna colSubject, ref CollisionResult colInfo);
}