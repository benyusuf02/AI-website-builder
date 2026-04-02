# 🚀 SaaS Production Deployment Guide (Universal)

This guide covers deployment via **Docker Compose (Recommended)** and **Manual Systemd**.

## 🏗️ Method 1: Docker Compose (Recommended)
This method isolates the application and handles SSL automatically.

### 1. Prerequisites
- Server with **Docker** & **Docker Compose** installed.
  ```bash
  sudo apt-get update
  sudo apt-get install docker.io docker-compose -y
  ```

### 2. Setup
1. Clone the repository to your server.
2. Navigate to root: `cd /path/to/YDeveloper`
3. **Configure Secrets:**
   - Edit `YDeveloper/appsettings.Production.json`. (Use the template provided).
   - Ensure DB Connection String points to your external DB or a dockerized DB.
   - **Important:** If using a local DB on the host, use `host.docker.internal` (if supported) or the host's private IP.

### 3. Run
```bash
sudo docker-compose up --build -d
```
This starts:
- `ydeveloper_app`: The ASP.NET Core Application (Port 8080 internal).
- `caddy_server`: Reverse Proxy handling SSL (Ports 80/443 public).

### 4. Database Migration
To update the database schema from within the container:
```bash
sudo docker exec -it ydeveloper_app dotnet ef database update
```

---

## 🛠️ Method 2: Manual Systemd (Legacy)
Use this if you prefer running directly on the host OS (Ubuntu/Debian).

### 1. Prerequisites
- .NET 8 Runtime: `sudo apt-get install -y aspnetcore-runtime-8.0`
- Caddy: `sudo apt install caddy`

### 2. Build & Publish
Locally or on server:
```bash
dotnet publish -c Release -o /var/www/ydeveloper
```

### 3. Service Configuration
Create `/etc/systemd/system/ydeveloper.service`:
```ini
[Unit]
Description=YDeveloper SaaS
After=network.target

[Service]
WorkingDirectory=/var/www/ydeveloper
ExecStart=/usr/bin/dotnet /var/www/ydeveloper/YDeveloper.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```
Start service: `sudo systemctl enable --now ydeveloper.service`

### 4. Caddy Configuration
Copy `Caddyfile` to `/etc/caddy/Caddyfile` and reload.
*Note: For manual setup, ensure Caddyfile points to `localhost:5000`, NOT `ydeveloper:8080`.*

---

## ✅ Verification
1. Visit `https://ydeveloper.com`.
2. Check logs:
   - Docker: `docker logs -f ydeveloper_app`
   - Systemd: `journalctl -u ydeveloper.service -f`
3. Verify SSL: Caddy automatically obtains certificates for all domains.
