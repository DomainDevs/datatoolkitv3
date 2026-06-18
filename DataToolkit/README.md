DataToolkit

A Metadata-Driven Development Platform for .NET

DataToolkit is a platform designed to automate software development and database modernization through metadata analysis, code generation, scaffolding, and migration tooling.

Instead of manually creating repetitive layers and artifacts, DataToolkit analyzes your database structure and generates production-ready components following modern architectural practices.

What is DataToolkit?

DataToolkit is not:

An ORM
A Dapper replacement
A migration-only tool
A CRUD generator

DataToolkit is a development automation platform capable of generating complete backend solutions from database metadata.



Database
    ↓
Metadata Analysis
    ↓
DataToolkit
    ↓
┌─────────────────────────┐
│ Entities                │
│ Repositories            │
│ Services                │
│ APIs                    │
│ DTOs                    │
│ SQL Scripts             │
│ Documentation           │
│ Migration Assets        │
└─────────────────────────┘


Main Capabilities
Backend Scaffolding

Generate complete backend solutions following Clean Architecture principles.

Generated Artifacts
Domain Entities
Repository Interfaces
Repository Implementations
Application Services
DTOs
API Controllers
Dependency Injection
Validation Classes
Mapping Profiles

Database Migration

Accelerate migration projects through metadata-driven automation.

Features
Database Inventory
Schema Comparison
Mapping Work Files
Migration SQL Generation
Validation Scripts
Reconciliation Reports
Data Access Framework

Built-in lightweight runtime for generated solutions.

Components
Unit Of Work
Repository Pattern
Dapper Integration
Fluent Query Builder
Retry Policies
Transaction Management
Metadata Discovery

The foundation of the platform.

Discover and model:

Tables
Columns
Keys
Relationships
Constraints
Indexes

This metadata becomes the source for all generated artifacts.


Architecture
┌───────────────────────────┐
│      DataToolkit          │
└─────────────┬─────────────┘
              │
              ▼
┌───────────────────────────┐
│ Metadata Discovery Layer  │
└─────────────┬─────────────┘
              │
      ┌───────┼────────┐
      ▼       ▼        ▼

Code Gen  Migration  Documentation
      │       │        │
      ▼       ▼        ▼

Backend   WorkFiles   Reports


Use Cases
Rapid Backend Development

Generate complete backend projects in minutes.

Database
    ↓
DataToolkit
    ↓
Clean Architecture Solution
Legacy Modernization

Transform legacy databases into modern applications.

Database Migration Projects

Generate inventories, mappings, scripts, and migration artifacts.

Enterprise Development

Standardize architecture across multiple projects.

Vision

DataToolkit aims to become a unified platform for:

Backend Scaffolding
Development Automation
Database Modernization
Metadata Management
Migration Engineering

allowing teams to generate and maintain software solutions from a single source of truth: metadata.
