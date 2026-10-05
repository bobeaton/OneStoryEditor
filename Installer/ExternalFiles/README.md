# ExternalFiles

## MicrosoftEdgeWebview2Setup.exe

- Source: Microsoft's Evergreen WebView2 Runtime bootstrapper, downloaded from
  https://go.microsoft.com/fwlink/p/?LinkId=2124703 on 2026-10-04 (Authenticode-signed by Microsoft Corporation).
- It is the online bootstrapper (the "Evergreen Bootstrapper"): it needs internet access when it runs, and it installs
  the shared, auto-updating WebView2 runtime. Microsoft allows redistributing it with an app.
- The installer (`Product.wxs`) runs it silently (`/silent /install`) only when no WebView2 runtime is installed, and
  ignores a failure (for example when offline): the app then falls back to its IE panes.
- To refresh it, download the same link again and replace the file.
