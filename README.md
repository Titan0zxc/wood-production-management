# ⚫ WOOD PRODUCTION MANAGEMENT

> **Industrial Edition** | Система управления деревообрабатывающим предприятием

![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![WinForms](https://img.shields.io/badge/WinForms-00A4EF?style=for-the-badge&logo=windows&logoColor=white)

**Специализированная информационная система** для автоматизации полного производственного цикла деревообрабатывающего предприятия — от заказа клиента до отгрузки готовой продукции.

## 🚀 Функциональность

- **👥 Ролевая модель:** Клиент, Менеджер, Разработчик, Производитель, Тестировщик
- **📦 Управление заказами:** Полный цикл от создания до доставки
- **📊 Отслеживание статусов:** Реальный мониторинг выполнения каждого заказа
- **💬 Внутренняя переписка:** Интегрированная система коммуникации между отделами
- **📋 Документооборот:** Управление ТЗ, спецификациями, отчетностью
- **💰 Финансовый контроль:** Учет предоплат, неустоек, финальных расчетов

## 🛠 Технологический стек

*   **Backend:** C#, .NET Framework
*   **Frontend:** Windows Forms (WinForms)
*   **Data Storage:** File-based storage (TXT serialization)
*   **Modeling:** UML, BPMN, IDEF3, DFD
*   **Architecture:** 3-Layer (Presentation, Business Logic, Data Access)

## 📊 Диаграммы и проектирование

<details>
<summary><b>Диаграммы проектирования</b> (Нажми, чтобы развернуть)</summary>

| Диаграмма DFD | Диаграмма IDEF3 |
| :---: | :---: |
| <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Диаграмма_DFD.png" width="400"/> | <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Диаграмма_IDEF3.png" width="400"/> |

| Диаграмма вариантов использования | Диаграмма классов |
| :---: | :---: |
| <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Диаграмма_вариантов_использования.png" width="400"/> | <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Диаграмма_классов.png" width="400"/> |

| Диаграмма взаимодействия | Диаграмма последовательности |
| :---: | :---: |
| <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Диаграмма_взаимодействия_участников_компании.png" width="1400"/> | <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Диаграмма_последовательности.png" width="400"/> |


| Диаграмма узлов | Исполняемая модель бизнес-процесса |
| :---: | :---: |
| <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Диаграмма_узлов.png" width="1200"/> | <img src="https://github.com/Titan0zxc/wood-production-management/raw/main/screenshots/Исполняемая_модель_бизнес_процесса.png" width="1200"/> |



</details>

## 🏗️ Архитектура

Проект реализован по трехслойной архитектуре:

1.  **Presentation Layer:** Windows Forms приложение
2.  **Business Logic Layer:** Сервисы и менеджеры (OrderService, TaskService)
3.  **Data Access Layer:** Модели данных и файловое хранилище

## 📦 Установка и запуск

1.  **Клонируйте репозиторий:**
    ```bash
    git clone https://github.com/Titan0zxc/wood-production-management.git
    ```

2.  **Запустите приложение:**
    *   Откройте `Система_для_предприятия.sln` в **Visual Studio**
    *   Соберите и запустите решение (F5)

*Примечание: Система использует файловое хранение данных — дополнительные настройки не требуются*

## 📄 Документация

Полная техническая документация проекта доступна в файле:
- [специализированная_информационная_система_для_предприятия.docx](https://github.com/Titan0zxc/wood-production-management/raw/main/docs/специализированная_информационная_система_для_предприятия.docx)

## 👨‍💻 Разработчик

**Евгений** - Начинающий .NET разработчик

*   📧 Почта: [titanozxc@gmail.com](mailto:titanozxc@gmail.com)
*   💻 GitHub: [Titan0zxc](https://github.com/Titan0zxc)

## 🔗 Ссылки на другие проекты

*   🎓 [University Schedule System](https://github.com/Titan0zxc/university-schedule-system) - Система управления расписанием вуза

---
*Проект разработан в учебных целях. Демонстрирует полный цикл разработки информационной системы.*
