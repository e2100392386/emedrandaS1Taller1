namespace emedrandaS1Taller1.Views;

using System.Text;
using System.Text.RegularExpressions;
public partial class Inscripcion : ContentPage
{
	public Inscripcion()
	{
		InitializeComponent();
	}

    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        // 1) Lectura
        var tipoId = TipoIdPicker.SelectedItem?.ToString() ?? "";
        var numeroId = (NumeroIdEntry.Text ?? "").Trim();
        var apPat = (ApellidoPaternoEntry.Text ?? "").Trim();
        var apMat = (ApellidoMaternoEntry.Text ?? "").Trim();
        var nombres = (NombresEntry.Text ?? "").Trim();
        var telefono = (TelefonoEntry.Text ?? "").Trim();
        var correo = (CorreoEntry.Text ?? "").Trim();
        var correo2 = (ConfirmarCorreoEntry.Text ?? "").Trim();
        var carrera = CarreraPicker.SelectedItem?.ToString() ?? "";
        var modalidad = ModalidadPicker.SelectedItem?.ToString() ?? "";
        var campus = CampusPicker.SelectedItem?.ToString() ?? "";

        // 2) Validaciones mínimas
        // Requeridos
        if (string.IsNullOrWhiteSpace(tipoId) || string.IsNullOrWhiteSpace(numeroId) ||
            string.IsNullOrWhiteSpace(apPat) || string.IsNullOrWhiteSpace(apMat) ||
            string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(telefono) ||
            string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(carrera) ||
            string.IsNullOrWhiteSpace(modalidad) || string.IsNullOrWhiteSpace(campus))
        {
            await DisplayAlert("Faltan datos", "Completa todos los campos obligatorios.", "OK");
            return;
        }

        // Solo letras (incluye espacios y acentos)
        var letrasRegex = new Regex(@"^[\p{L}\p{M} '\-]+$"); // letras, espacios, apóstrofe, guion
        if (!letrasRegex.IsMatch(apPat) || !letrasRegex.IsMatch(apMat) || !letrasRegex.IsMatch(nombres))
        {
            await DisplayAlert("Formato inválido", "Apellidos y nombres deben contener solo letras.", "OK");
            return;
        }

        // Correo válido y coincidente
        if (!EsEmailValido(correo))
        {
            await DisplayAlert("Correo inválido", "Ingresa un correo válido.", "OK");
            return;
        }
        if (!correo.Equals(correo2, StringComparison.OrdinalIgnoreCase))
        {
            await DisplayAlert("Correos no coinciden", "El correo y su confirmación deben ser iguales.", "OK");
            return;
        }

        // 3) Guardar en TXT (AppDataDirectory)
        var sb = new StringBuilder();
        sb.AppendLine($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Identificación: {tipoId} - {numeroId}");
        sb.AppendLine($"Apellidos: {apPat} {apMat}");
        sb.AppendLine($"Nombres: {nombres}");
        sb.AppendLine($"Correo: {correo}");
        sb.AppendLine($"Carrera: {carrera}");
        sb.AppendLine($"Modalidad: {modalidad}");
        sb.AppendLine($"Campus: {campus}");

        var folder = FileSystem.AppDataDirectory;
        var filePath = Path.Combine(folder, $"inscripcion_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

        await DisplayAlert(
            "Inscripción enviada",
            $"Se guardó el archivo:\n{filePath}",
            "OK");
    }

    private static bool EsEmailValido(string email)
    {
        // RegEx simple y suficiente para validación básica
        return Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
    }
}

/* ===== Behaviors simples ===== */

public class NumericOnlyBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.TextChanged += OnTextChanged;
    }
    protected override void OnDetachingFrom(Entry entry)
    {
        base.OnDetachingFrom(entry);
        entry.TextChanged -= OnTextChanged;
    }
    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;
        var filtered = new string((e.NewTextValue ?? "").Where(char.IsDigit).ToArray());
        if (entry.Text != filtered)
            entry.Text = filtered;
    }
}

public class LettersOnlyBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.TextChanged += OnTextChanged;
    }
    protected override void OnDetachingFrom(Entry entry)
    {
        base.OnDetachingFrom(entry);
        entry.TextChanged -= OnTextChanged;
    }
    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;

        // Acepta letras (incluye acentos), espacios, apóstrofe y guion
        var filtered = new string((e.NewTextValue ?? "")
            .Where(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '\'' || c == '-')
            .ToArray());

        if (entry.Text != filtered)
            entry.Text = filtered;
    }
}