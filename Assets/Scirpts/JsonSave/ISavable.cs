public interface ISavable
{
    void LoadData(Database data);
    void SaveData(ref Database data); // ref 찾아보기
}