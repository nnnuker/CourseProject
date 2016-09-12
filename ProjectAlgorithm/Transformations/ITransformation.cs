using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Transformations
{
    public interface ITransformation
    {
        ICompositeObject MoveObject(ICompositeObject compositeObject, float moveX, float moveY, float moveZ);
        ICompositeObject RotateObject(ICompositeObject compositeObject, float angleX, float angleY, float angleZ);
        ICompositeObject ScaleObject(ICompositeObject compositeObject, float scaleX, float scaleY, float scaleZ);
    }
}