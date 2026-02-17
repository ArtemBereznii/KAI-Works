using Domain;

namespace FileLibrary
{
    public interface IFileHandler
    {
        void SavePerson(Person person);
        Person[] LoadAllPersons();
    }
}
