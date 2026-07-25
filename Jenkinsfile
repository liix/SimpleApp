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

        stage ('Quality and Testing'){
            parallel {
                stage('Test') {
                    steps {
                        echo '=== Stage 3: Tests ==='
                        catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                            sh '''
                                dotnet test --no-build -c Release \
                                --logger "trx;LogFileName=test_results.trx" \
                                --collect:"XPlat Code Coverage" \
                                --results-directory ./TestResults \
                                -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
                            '''
                        }
                    }
                    post {
                        always {
                            mstest testResultsFile: '**/test_results.trx', keepLongStdio: true

                            recordCoverage(
                                tools: [[parser: 'COBERTURA', pattern: '**/coverage.cobertura.xml']],
                                id: 'cobertura',
                                name: 'Code Coverage',
                                sourceCodeRetention: 'LAST_BUILD'
                            )
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
                    docker run -d --name my-web-api -p ${port}:8080 my-web-api
                """
                
                echo 'The App is ready'
            }
        }
        
        stage('Check application') {
            steps {
                echo '=== Stage 5: Check ==='
                sh """
                    for i in \$(seq 1 30); do
                    if curl -fsS "http://localhost:${port}" >/dev/null; then
                        echo "App is up"
                        curl -fsS "http://localhost:${port}"
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
