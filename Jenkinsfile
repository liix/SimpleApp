pipeline {
    agent any
    triggers { pollSCM('H/5 * * * *') }
    stages {
        stage('Smoke') {
            steps {
                sh 'pwd; ls; docker version; dotnet --version || true'
            }
        }
    }
}