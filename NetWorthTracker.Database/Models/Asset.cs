using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetWorthTracker.Database.Models;

public class Asset : INotifyPropertyChanged
{
    public int Id { get; set; }
    
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }
    
    public int EntryId { get; set; }
    
    private decimal _value = 0;
    public decimal Value
    {
        get => _value;
        set
        {
            _value = value;
            OnPropertyChanged();
        }
    }
    
    public virtual Entry Entry { get; set; } = new Entry();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
