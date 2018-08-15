project = File.basename(File.expand_path("."))
guest_home = "/home/vagrant"
host_home = File.expand_path("~")

Vagrant.configure("2") do |config|
    config.vm.box = "ubuntu/xenial64"
    config.vm.box_check_update = false
    config.vm.provider "virtualbox" do |vb|
      vb.memory = "2048" # 2GB
      vb.cpus = 2
      vb.gui = true
    end

    config.vm.hostname = "VagrantTest"
    config.vm.synced_folder ".", "#{guest_home}/#{project}", owner: "vagrant", group: "vagrant"
    config.vm.synced_folder ".", "/vagrant", disabled: true
    config.vm.network :forwarded_port, guest: 80, host: 8000 # HTTP
    config.vm.network :forwarded_port, guest: 1433, host: 1433 # SQLServer
    config.vm.network :forwarded_port, guest: 5000, host: 5000 # Kestral (dotnet dev http)
    config.vm.network :forwarded_port, guest: 5001, host: 5001 # Kestral (dotnet dev https)

    config.vm.provision :shell, inline: <<-SHELL
        MSSQL_SA_PASSWORD='Om33z9d2l4d456a'
        MSSQL_PID='evaluation'
        SQL_INSTALL_AGENT='y'

        if [ -z $MSSQL_SA_PASSWORD ]
            then
              echo Environment variable MSSQL_SA_PASSWORD must be set for unattended install
              exit 1
        fi

        echo Adding Microsoft repositories...
        sudo curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -

        repoargs="$(curl https://packages.microsoft.com/config/ubuntu/16.04/mssql-server-2017.list)"
        sudo add-apt-repository "${repoargs}"

        repoargs="$(curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list)"
        sudo add-apt-repository "${repoargs}"

        echo Running apt-get update -y...
        sudo apt-get update -y

        echo Installing SQL Server...
        sudo apt-get install -y mssql-server

        echo Running mssql-conf setup...
        sudo MSSQL_SA_PASSWORD=$MSSQL_SA_PASSWORD MSSQL_PID=$MSSQL_PID /opt/mssql/bin/mssql-conf -n setup accept-eula

        echo Installing mssql-tools and unixODBC developer...
        sudo ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev

        echo Adding SQL Server tools to your path...
        echo PATH="$PATH:/opt/mssql-tools/bin" >> ~/.bash_profile
        echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc

        # Optional SQL Server Agent installation:
        if [ ! -z $SQL_INSTALL_AGENT ]
            then
            echo Installing SQL Server Agent...
            sudo apt-get install -y mssql-server-agent
        fi

        # Configure firewall to allow TCP port 1433:
        echo Configuring UFW to allow traffic on port 1433...
        sudo ufw allow 1433/tcp
        sudo ufw reload

        # Restart SQL Server after installing:
        echo Restarting SQL Server...
        sudo systemctl restart mssql-server

        # Connect to server and get the version:
        counter=1
        errstatus=1
        while [ $counter -le 5 ] && [ $errstatus = 1 ]
        do
        echo Waiting for SQL Server to start...
        sleep 3s
        /opt/mssql-tools/bin/sqlcmd \
            -S localhost \
            -U SA \
            -P $MSSQL_SA_PASSWORD \
            -Q "SELECT @@VERSION" 2>/dev/null
        errstatus=$?
        ((counter++))
        done

        # Display error if connection failed:
        if [ $errstatus = 1 ]
            then
            echo Cannot connect to SQL Server, installation aborted
            exit $errstatus
        fi

        # Creating the tool symlink:
        ln -sfn /opt/mssql-tools/bin/sqlcmd /usr/bin/sqlcmd

        sqlcmd -S localhost -U SA -P $MSSQL_SA_PASSWORD -Q "SELECT Name from sys.Databases"

        echo 'Installing dotnet core'
        wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
        dpkg -i packages-microsoft-prod.deb
        apt-get -q -y install apt-transport-https
        apt-get update
        apt-get -q -y install aspnetcore-runtime-2.1
        apt-get -q -y install dotnet-sdk-2.1

        # -q -y means quietly install and assume yes to all prompts

        # Start running the dev server on the code
        cd $project
        dotnet watch run

    SHELL

end