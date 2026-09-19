pipeline {
    agent any
    triggers { pollSCM('H/5 * * * *') }

    parameters {
        string(name: 'PORT', defaultValue: '8081')
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

        stage ('Quality and Testing'){
            parallel {
                stage('Tests') {
                    steps {
                        echo '=== Stage 3: Tests ==='
                        catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                            sh '''dotnet test --no-build -c Release'''
                        }
                    }
                }

                stage('Code Style Check') {
                    steps {
                        echo '=== Checking Code Formatting ==='
                        sh 'dotnet format --verify-no-changes'
                    }
                }
            }
        }

        stage('Deploy (Publish)') {
            steps {
                echo '=== Stage 4: Deploy ==='
                sh """ 
                    docker build -t my-web-api .
                    docker stop my-web-api || true
                    docker rm my-web-api || true
                    docker run -d --name my-web-api -p ${params.PORT}:8080 my-web-api
                """
                
                echo 'The App is ready'
            }
        }
        
        stage('Check application') {
            steps {
                echo '=== Stage 5: Check ==='
                sh """
                    for i in \$(seq 1 30); do
                    if curl -fsS "http://localhost:${params.PORT}" >/dev/null; then
                        echo "App is up"
                        curl -fsS "http://localhost:${params.PORT}"
                        exit 0
                    fi
                    echo "Waiting... (\$i)"
                    sleep 2
                    done
                    echo "App did not become ready in time"
                    exit 1
                """
            }
        }
    }

    post {
        always {
            echo 'Cleaning...'
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
