namespace Tennis.DAO
{
    public abstract class DAO<T>
    {
        public abstract bool Create(T obj);
        public abstract bool Delete(T obj);
        public abstract bool Update(T obj);
        public abstract T Find(int id);

    }
}
