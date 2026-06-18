# DataToolkit

> **Generate Applications. Automate Migrations. Accelerate Delivery.**

DataToolkit is a metadata-driven development platform for .NET that helps teams automate software development, backend scaffolding, database modernization, and migration projects.

Instead of manually creating repetitive code and migration artifacts, DataToolkit analyzes database metadata and generates production-ready assets following modern architectural practices.

---

# Overview

Most tools solve a single problem:

* ORMs generate entities.
* Scaffolding tools generate CRUDs.
* Migration tools move data.
* Documentation tools generate technical documents.

DataToolkit unifies these capabilities through a metadata-driven approach.

```text
Database
    ↓
Metadata Discovery
    ↓
DataToolkit
    ↓
┌─────────────────────────────┐
│ Entities                    │
│ Repositories                │
│ Services                    │
│ APIs                        │
│ DTOs                        │
│ SQL Scripts                 │
│ Documentation               │
│ Migration Assets            │
└─────────────────────────────┘
```

---

# What is DataToolkit?

DataToolkit is not:

* An ORM
* A Dapper replacement
* A migration-only tool
* A simple CRUD generator

DataToolkit is a development automation platform capable of generating complete backend solutions and migration assets from database metadata.

---

# Core Principles

## Metadata-Driven

Everything starts from metadata.

Database structures become the source of truth used to generate software artifacts and migration assets.

## Lightweight

Built around proven technologies such as Dapper and ADO.NET.

## Automation First

Reduce repetitive development tasks and accelerate delivery.

## Provider Agnostic

Designed to support multiple database engines and environments.

## Enterprise Ready

Support for transactions, resiliency, telemetry, and large-scale migration initiatives.

---

# Platform Components

## DataToolkit.Library

Runtime framework providing:

* Connection Management
* Unit Of Work
* SQL Execution
* Repository Pattern
* Metadata Discovery
* Fluent Query Builder
* Retry Policies
* Transaction Management

```text
DataToolkit.Library
│
├── Connections
├── Providers
├── UnitOfWork
├── SqlExecutor
├── Metadata
├── Fluent Queries
└── Resilience
```

---

## DataToolkit.Builder

Code generation and automation engine.

Generates:

* Domain Entities
* Repository Interfaces
* Repository Implementations
* Services
* DTOs
* API Controllers
* Dependency Injection
* Validation Components
* SQL Scripts
* Documentation
* Migration Artifacts

```text
Database
    ↓
Builder
    ↓
Generated Solution
```

---

## Migration Toolkit

Provides tooling for database modernization and migration projects.

Capabilities include:

### Inventory

Analyze source and target databases.

### Comparison

Detect schema differences.

### Mapping

Generate migration work files.

### SQL Generation

Create migration scripts automatically.

### Validation

Generate reconciliation and verification scripts.

### Documentation

Produce migration reports and technical documentation.

---

# Backend Scaffolding

Generate complete backend solutions following Clean Architecture principles.

Example generated structure:

```text
Customer
│
├── Customer.cs
├── ICustomerRepository.cs
├── CustomerRepository.cs
├── CustomerService.cs
├── CustomerDto.cs
└── CustomerController.cs
```

Supported layers include:

* Domain
* Application
* Infrastructure
* API
* Shared Components

---

# Metadata Discovery

DataToolkit discovers and models:

* Tables
* Columns
* Primary Keys
* Foreign Keys
* Constraints
* Indexes
* Relationships

This metadata becomes the foundation for all generated artifacts.

---

# Architecture

```text
┌──────────────────────────────┐
│      DataToolkit.Builder     │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│      DataToolkit.Library     │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│ Database Providers           │
│ SQL Server                   │
│ PostgreSQL                   │
│ MySQL                        │
│ Sybase                       │
└──────────────────────────────┘
```

---

# Features

## Development

* Backend Scaffolding
* Clean Architecture Generation
* Repository Generation
* Service Generation
* API Generation
* DTO Generation

## Data Access

* Unit Of Work
* Repository Pattern
* Dapper Integration
* Fluent Queries
* Dynamic SQL Generation

## Metadata

* Database Inventory
* Schema Discovery
* Relationship Analysis
* Metadata Modeling

## Migration

* Inventory
* Comparison
* Mapping
* Work Files
* SQL Generation
* Validation Scripts

---

# Sample Usage

## Service Registration

```csharp
builder.Services.AddDataToolkit(
    builder.Configuration);
```

## Unit Of Work

```csharp
using var uow = serviceProvider
    .GetRequiredService<IUnitOfWork>();

var customers = await uow.Sql
    .QueryAsync<Customer>(
        "SELECT * FROM Customers");
```

---

# Use Cases

## Rapid Backend Development

Generate complete backend solutions from existing databases.

## Enterprise Standardization

Apply consistent architectural patterns across projects.

## Legacy Modernization

Accelerate modernization of legacy systems and databases.

## Database Migration

Generate migration inventories, mappings, scripts, and validation artifacts.

## Development Automation

Reduce manual coding effort and improve consistency.

---

# Roadmap

## Version 1

* Data Access Framework
* Metadata Discovery
* Backend Scaffolding
* Migration Builder

## Version 2

* Bulk Operations
* Advanced Templates
* Documentation Generator
* Validation Framework

## Version 3

* Visual Designer
* ETL Integration
* Multi-Database Workflows
* AI-Assisted Mapping

---

# Vision

DataToolkit aims to become a unified platform for:

* Backend Scaffolding
* Development Automation
* Metadata Management
* Database Modernization
* Migration Engineering

allowing teams to generate and maintain software solutions from a single source of truth: metadata.

---

# Why DataToolkit?

| Capability                    | DataToolkit | Dapper | EF Core |
| ----------------------------- | ----------- | ------ | ------- |
| Data Access                   | ✅           | ✅      | ✅       |
| Metadata Discovery            | ✅           | ❌      | Partial |
| Backend Scaffolding           | ✅           | ❌      | Partial |
| Clean Architecture Generation | ✅           | ❌      | ❌       |
| Migration Inventory           | ✅           | ❌      | ❌       |
| Work File Generation          | ✅           | ❌      | ❌       |
| SQL Generation                | ✅           | ❌      | Partial |
| Development Automation        | ✅           | ❌      | ❌       |

---

## From Metadata to Software.
