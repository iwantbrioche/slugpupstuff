
namespace SlugpupStuff.PupsPlusCustom
{
    public class GiftedItem
    {
        public AbstractPhysicalObject item;
        public int age;
        public float score;
        public GiftedItem(AbstractPhysicalObject gift, float s)
        {
            age = 0;
            item = gift;
            score = s;
        }
        public void Update()
        {
            age++;
        }
    }
}
