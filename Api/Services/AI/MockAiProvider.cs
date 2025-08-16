using LegalStrategyAdvisor.Api.Services.AI;

namespace LegalStrategyAdvisor.Api.Services.AI;

public class MockAiProvider : IAiProvider
{
    private readonly ILogger<MockAiProvider> _logger;

    public string ProviderName => "Mock";

    public MockAiProvider(ILogger<MockAiProvider> logger)
    {
        _logger = logger;
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock AI Provider generating response for prompt of length: {Length}", prompt.Length);

        // Simulate some processing time
        await Task.Delay(Random.Shared.Next(500, 1500), cancellationToken);

        // Generate a structured mock response based on the prompt content
        var response = GenerateStructuredMockResponse(prompt);

        _logger.LogDebug("Mock AI Provider generated response of length: {Length}", response.Length);
        return response;
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        // Mock provider is always available
        return Task.FromResult(true);
    }

    private static string GenerateStructuredMockResponse(string prompt)
    {
        var promptLower = prompt.ToLowerInvariant();

        // Detect the type of legal query based on keywords
        if (promptLower.Contains("contract") || promptLower.Contains("agreement"))
        {
            return GenerateContractAdvice(prompt);
        }

        if (promptLower.Contains("criminal") || promptLower.Contains("defendant") || promptLower.Contains("prosecution"))
        {
            return GenerateCriminalLawAdvice(prompt);
        }

        if (promptLower.Contains("civil rights") || promptLower.Contains("discrimination") || promptLower.Contains("constitutional"))
        {
            return GenerateCivilRightsAdvice(prompt);
        }

        if (promptLower.Contains("corporate") || promptLower.Contains("business") || promptLower.Contains("merger"))
        {
            return GenerateCorporateLawAdvice(prompt);
        }

        // General legal strategy response
        return GenerateGeneralLegalAdvice(prompt);
    }

    private static string GenerateContractAdvice(string prompt)
    {
        return @"## Contract Law Analysis

### Key Considerations:
• **Contract Formation**: Ensure all essential elements are present (offer, acceptance, consideration, capacity, legality)
• **Terms Review**: Examine express and implied terms for ambiguity or unfairness
• **Performance Obligations**: Clarify each party's duties and timelines
• **Risk Allocation**: Analyze limitation clauses, indemnification, and force majeure provisions

### Recommended Strategy:
1. **Due Diligence**: Conduct thorough review of all contract terms and conditions
2. **Negotiation Points**: Focus on key commercial terms and risk allocation
3. **Compliance Framework**: Establish monitoring mechanisms for ongoing obligations
4. **Dispute Resolution**: Include clear escalation procedures and alternative dispute resolution

### Relevant Precedents:
• Contract interpretation follows objective reasonable person standard
• Good faith and fair dealing obligations apply to all contracts
• Material breach analysis considers timing, severity, and cure possibilities

### Next Steps:
- [ ] Complete contract term analysis
- [ ] Identify potential areas of dispute
- [ ] Develop risk mitigation strategies
- [ ] Prepare negotiation strategy document";
    }

    private static string GenerateCriminalLawAdvice(string prompt)
    {
        return @"## Criminal Law Strategy

### Constitutional Protections:
• **Fourth Amendment**: Search and seizure protections
• **Fifth Amendment**: Self-incrimination and due process rights  
• **Sixth Amendment**: Right to counsel and confrontation of witnesses
• **Eighth Amendment**: Prohibition against cruel and unusual punishment

### Defense Strategy Framework:
1. **Evidence Analysis**: Challenge admissibility of prosecution evidence
2. **Witness Preparation**: Develop comprehensive witness examination strategy
3. **Procedural Defenses**: Identify potential procedural violations
4. **Plea Considerations**: Evaluate risks and benefits of plea negotiations

### Key Precedents:
• Miranda v. Arizona: Rights during custodial interrogation
• Mapp v. Ohio: Exclusionary rule for illegally obtained evidence
• Gideon v. Wainwright: Right to effective assistance of counsel

### Investigation Priorities:
- [ ] Review arrest and search procedures
- [ ] Analyze chain of custody for physical evidence
- [ ] Interview potential witnesses
- [ ] Research applicable statutory defenses
- [ ] Prepare motion practice calendar";
    }

    private static string GenerateCivilRightsAdvice(string prompt)
    {
        return @"## Civil Rights Legal Strategy

### Constitutional Framework:
• **Equal Protection Clause**: Fourteenth Amendment analysis
• **Due Process Rights**: Procedural and substantive protections
• **First Amendment**: Speech, religion, and assembly rights
• **Section 1983 Claims**: Civil rights violations under color of law

### Strategic Approach:
1. **Discrimination Analysis**: Apply appropriate level of scrutiny (strict, intermediate, rational basis)
2. **Damages Assessment**: Calculate compensatory and punitive damages
3. **Injunctive Relief**: Seek prospective relief to prevent ongoing violations
4. **Class Action Potential**: Evaluate systemic discrimination claims

### Landmark Cases:
• Brown v. Board of Education: Separate is not equal doctrine
• Loving v. Virginia: Fundamental right to marriage
• Tennessee v. Garner: Use of deadly force standards

### Action Plan:
- [ ] Document pattern and practice of discrimination
- [ ] Gather statistical evidence of disparate impact
- [ ] Interview affected individuals and witnesses
- [ ] Research applicable federal and state civil rights statutes
- [ ] Develop comprehensive remedy framework";
    }

    private static string GenerateCorporateLawAdvice(string prompt)
    {
        return @"## Corporate Law Strategy

### Governance Framework:
• **Fiduciary Duties**: Directors' duty of care and loyalty obligations
• **Shareholder Rights**: Voting, information access, and derivative suit rights
• **Compliance Requirements**: SEC reporting, SOX compliance, state law obligations
• **Transaction Structure**: Mergers, acquisitions, and reorganization considerations

### Risk Management:
1. **Due Diligence**: Comprehensive financial, legal, and operational review
2. **Regulatory Compliance**: Industry-specific requirements and oversight
3. **Contract Management**: Key vendor, customer, and employment agreements
4. **Intellectual Property**: Patent, trademark, and trade secret protection

### Key Legal Principles:
• Business Judgment Rule: Protection for informed board decisions
• Entire Fairness Standard: Self-dealing transaction scrutiny
• Revlon Duties: Sale of control obligations

### Implementation Steps:
- [ ] Corporate governance audit and policy review
- [ ] Board and committee structure optimization
- [ ] Stakeholder communication strategy
- [ ] Regulatory compliance assessment
- [ ] Transaction documentation and approval process";
    }

    private static string GenerateGeneralLegalAdvice(string prompt)
    {
        return @"## Legal Strategy Analysis

### Initial Assessment:
Based on the provided information, this matter requires comprehensive legal analysis across multiple areas of law. The following framework provides a structured approach to developing an effective legal strategy.

### Strategic Framework:
1. **Legal Research**: Conduct thorough analysis of applicable statutes, regulations, and case law
2. **Fact Development**: Gather and organize all relevant evidence and documentation
3. **Risk Assessment**: Identify potential legal exposures and mitigation strategies
4. **Alternative Solutions**: Explore negotiation, mediation, and settlement opportunities

### Key Considerations:
• **Jurisdictional Issues**: Determine proper venue and applicable law
• **Statute of Limitations**: Ensure timely filing of all claims and defenses
• **Evidence Preservation**: Implement litigation hold and discovery strategy
• **Client Objectives**: Align legal strategy with business and personal goals

### Recommended Actions:
- [ ] Complete comprehensive legal research
- [ ] Conduct client interview and fact-gathering
- [ ] Develop case timeline and procedural calendar
- [ ] Assess settlement and alternative dispute resolution options
- [ ] Prepare preliminary case strategy and budget

### Next Steps:
This analysis provides a foundation for more detailed strategy development. Additional consultation and research will be necessary to refine the approach based on specific factual and legal developments.

*Note: This analysis is provided for strategic planning purposes and does not constitute specific legal advice.*";
    }
}
