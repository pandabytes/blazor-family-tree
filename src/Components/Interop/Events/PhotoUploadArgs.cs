namespace Blazor.FamilyTreeJS.Components.Interop.Events;

/// <summary>
/// Contain data when a person's photo is uploaded.
/// </summary>
/// <param name="FileName">Name of the file being uploaded.</param>
/// <param name="FileStreamReference">Reference to the file stream from JavaScript.</param>
public record PhotoUploadArgs(string FileName, IJSStreamReference FileStreamReference);
