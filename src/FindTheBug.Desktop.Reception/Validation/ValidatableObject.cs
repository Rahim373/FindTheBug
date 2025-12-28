using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FindTheBug.Desktop.Reception.Validation;

/// <summary>
/// A wrapper for properties that require validation
/// </summary>
public class ValidatableObject<T> : INotifyPropertyChanged
{
    private T? _value;
    private string? _error;
    private bool _isValid = true;
    private bool _isDirty;

    private readonly List<ValidationRule<T>> _validationRules = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ValidatableObject()
    {
        Errors = new ObservableCollection<string>();
    }

    public List<ValidationRule<T>> ValidationRules => _validationRules;

    public T? Value
    {
        get => _value;
        set
        {
            if (Equals(_value, value))
                return;

            _value = value;
            _isDirty = true;
            Validate();
            OnPropertyChanged();
        }
    }

    public string? Error
    {
        get => _error;
        private set
        {
            if (_error != value)
            {
                _error = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsValid
    {
        get => _isValid;
        private set
        {
            if (_isValid != value)
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsDirty => _isDirty;

    public ObservableCollection<string> Errors { get; }

    public void AddValidationRule(ValidationRule<T> rule)
    {
        _validationRules.Add(rule);
    }

    public void ClearValidationRules()
    {
        _validationRules.Clear();
    }

    public bool Validate()
    {
        Errors.Clear();
        List<string> errors = new();

        foreach (var rule in _validationRules)
        {
            bool isValid = rule.Validate(_value, out string? errorMessage);
            if (!isValid)
            {
                errors.Add(errorMessage ?? "Invalid value");
            }
        }

        if (errors.Any())
        {
            Error = errors[0];
            foreach (var error in errors)
            {
                Errors.Add(error);
            }
            IsValid = false;
            return false;
        }
        else
        {
            Error = null;
            IsValid = true;
            return true;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ForceValidation()
    {
        _isDirty = true;
        Validate();
    }

    public void ClearErrors()
    {
        Errors.Clear();
        Error = null;
        IsValid = true;
    }
}