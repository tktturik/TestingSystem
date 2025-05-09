using System;
using System.Collections.Generic;
using System.Windows.Controls;
using testingSystem.Utilities;
using testingSystem.View.UserControls;
using testingSystem.ViewModels;

public class UserControlFactory
{
    private readonly Dictionary<Type, Func<UserControl>> _userControlMap;

    public UserControlFactory()
    {
        // Инициализируем словарь, где ключ - тип ViewModel, а значение - функция создания UserControl
        _userControlMap = new Dictionary<Type, Func<UserControl>>
        {
            { typeof(CreateTestVM), () => new CreateTest() },
            { typeof(TakingTestVM), () => new TakingTest() },
            { typeof(HomePageVM), () => new Home() },
            { typeof(ChoosingTestVM), () => new ChoosingTest() },
            { typeof(TasksVM), () => new Tasks() },
            { typeof(SettingsVM), () => new Settings() },
            { typeof(MyFilesVM), () => new MyFiles() },
            { typeof(PersonalAccountVM), () => new PersonalAccount() },
            { typeof(WhoISVM), () => new WhoIs() },
            { typeof(ResultTestVM), () => new TestResult() }

        };
    }

    public UserControl CreateUserControl(ViewModelBase viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        // Получаем тип ViewModel
        var viewModelType = viewModel.GetType();

        // Проверяем, есть ли соответствующий UserControl в словаре
        if (_userControlMap.TryGetValue(viewModelType, out var userControlFactory))
        {
            // Создаем UserControl и устанавливаем DataContext
            var userControl = userControlFactory();
            userControl.DataContext = viewModel;
            return userControl;
        }

        throw new InvalidOperationException($"No UserControl registered for ViewModel type: {viewModelType}");
    }
}