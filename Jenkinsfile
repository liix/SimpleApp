pipeline {
    agent any

    parameters {
        string(name: 'REPORT_USER', defaultValue: 'vasia')
        string(name: 'REPORT_PERIOD', defaultValue: '1')
        choice(name: 'REPORT_FORMAT', choices: ['txt', 'md'])
    }
    
    stages {
        
        stage('Restore Dependencies') {
            steps {
                echo '=== Stage 1: Restore Dependencies ==='
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo '=== Stage 2: Build ==='
                sh 'dotnet build --no-restore -c Release'
            }
        }

        stage('Test') {
            steps {
                echo '=== Stage 3: Tests ==='
                catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                    sh 'dotnet test --no-build -c Release'
                }
            }
        }

        stage('Deploy (Publish)') {
            steps {
                echo '=== Stage 4: Deploy ==='
                writeFile file: 'Dockerfile', text: params.dockerfile
                sh """ 
                    docker build -t my-web-api .
                    docker stop my-web-api || true
                    docker rm my-web-api || true
                    docker run -d --name my-web-api -p ${port}:8080 my-web-api
                """
                
                echo 'The App is ready'
            }
        }
        
        stage('Check application') {
            steps {
                echo '=== Stage 5: Check ==='
                sh """curl http://localhost:${port}"""
            }
        }
        
        stage('Execute DSL') {
            steps {
                echo '=== Stage 6: Generate report ==='
                writeFile file: 'generate_report.cs', text: params.generate_report
                sh "dotnet run generate_report.cs -- ${params.REPORT_USER} ${params.REPORT_PERIOD} ${params.REPORT_FORMAT} ${port}"
            }
        }
    }

    post {
        always {
            echo 'Cleaning...'
            cleanWs()
        }
        success {
            echo 'Pipeline finished successfully'
        }
        failure {
            echo 'The build failed!'
        }
        unstable {
            echo 'The build is unstable!'
        }
    }
}
