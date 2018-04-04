using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mdOrganizer.Services
{
    public interface IFileSystem
    {
        //Создает новый файл и делает его текущим
        Task CreateDocumentAsync();
        //Клонирует текущий файл
        Task CloneDocumentAsync();
        //Переключает текущий файл
        Task SwitchDocumentAsync();
        //Сохраняет текущий файл
        Task SaveDocumentAsync();
        //Загружает текущий файл
        Task ReadDocumentAsync();

        Task ExitAsync();
    }
}
